using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate;

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример расширения функционала сущности до моба
    /// </summary>
    public abstract class Mob : NetEntity
    {
        /// <summary>
        /// здоровье
        /// </summary>
        [GalaxyVar(1)]
        public int heal = 100;
        /// <summary>
        /// текущее состояние
        /// </summary>
        [GalaxyVar(2)]
        public byte state;
        /// <summary>
        /// Максимальное здоровье
        /// </summary>
        [GalaxyVar(3)]
        public int maxHeal = 100;
        /// <summary>
        /// Какому спавнеру пренадлежит моб
        /// </summary>
        public MobSpawner spawner;
        /// <summary>
        /// Множитель скорости движения моба
        /// </summary>
        public float moveSpeed = 0.1f;
        /// <summary>
        /// Точка в которую движется моб
        /// </summary>
        internal GalaxyVector3 movePoint;
        /// <summary>
        /// Дистанция агро
        /// </summary>
        internal int agroDistance = 20;
        /// <summary>
        /// Время между атаками
        /// </summary>
        internal float attackDelay = 2;
        /// <summary>
        /// Дистанция атаки (квадат дистанции)
        /// </summary>
        internal float attackDistanse = 2;
        /// <summary>
        /// Минимальный урон
        /// </summary>
        internal int minDamage = 1;
        /// <summary>
        /// Максимальный урон
        /// </summary>
        internal int maxDamage = 5;
        /// <summary>
        /// Цель за которой бегает моб
        /// </summary>
        internal RPGTemplatePlayer target;
        /// <summary>
        /// Счетчик атак
        /// </summary>
        internal int attackCount;
        /// <summary>
        /// Время последней атаки
        /// </summary>
        float lastAttack;


        public Mob(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {

        }
        /// <summary>
        /// Просим случайную точку в пределах текущего спавнера
        /// </summary>
        /// <returns></returns>
        public GalaxyVector3 RandomPoint()
        {
            if (spawner == null) return new GalaxyVector3();
            movePoint.y = spawner.position.y;
            movePoint.x = GRand.NextInt((int)spawner.position.x - spawner.zoneSize, (int)spawner.position.x + spawner.zoneSize);
            movePoint.z = GRand.NextInt((int)spawner.position.z - spawner.zoneSize, (int)spawner.position.z + spawner.zoneSize);
            return movePoint;
        }
        /// <summary>
        /// Двигаемся в заранее созданную случайную точку
        /// </summary>
        public void RandomMove()
        {
            if (state > 3) return;
            if ((transform.position - movePoint).SqrMagnitude > 2)
            {
                state = (byte)MobState.move;

                //      transform.position = GalaxyVector3.Lerp(transform.position, movePoint, instance.Time.deltaTime * moveSpeed);
                transform.position = GalaxyVector3.Move(transform.position, movePoint, 1, instance.Time.deltaTime);
            }
            else
            {
                state = (byte)MobState.idle;
            }
        }
        /// <summary>
        /// Атакуем или агримся
        /// </summary>
        public void Attack()
        {
            if (state >= 20) return;
            // выходим если нет цели
            if (target == null) return;
            // выходим если цель мертва
            if (target.heal <= 0)
            {
                target = null;
                state = 0;
                return;
            }
            float sqrDis = (target.transform.position - transform.position).SqrMagnitude;
            // выходим если цель ушла за предел агро зоны
            if (sqrDis > agroDistance * 2)
            {
                target = null;
                state = 0;
                return;
                attackCount = 0;
            }
            //Если игрок далеко, идем к нему
            if (sqrDis > attackDistanse)
            {
                state = (byte)MobState.follow;
                transform.position = GalaxyVector3.Lerp(transform.position, target.transform.position, instance.Time.deltaTime * moveSpeed * 2);
                return;
            }

            // выдерживаем паузу между атаками
            if (instance.Time.time - lastAttack < attackDelay) return;
            lastAttack = instance.Time.time;
            // проверяем не пытался ли игрок подменить атаку
            attackCount++;
            if (!OnAttack()) return;
            state = (byte)MobState.attack;
            // рандомим дамаг
            int damage = GRand.NextInt(minDamage, maxDamage);
            BitGalaxy message = new BitGalaxy();
            message.WriteValue(damage);
            // отправляем сообщение о том что мы наносим кому то дамаг. 
            SendMessage((byte)MobState.attack, message.data, GalaxyDeliveryType.reliable);
            // сообщяем игроку о уроне
            target.SetDamage(damage);


        }

        public abstract bool OnAttack();

        public void Death()
        {
            state = (byte)MobState.death;
            Invoke("Remove", 5);
        }

        public void Remove()
        {
            Destory();
        }

        public void RemoveInSpawner()
        {
            spawner.Remove(this);
        }
        // сюда попадают игроки оказавшиеся рядом с мобом
        public void PlayerNear(RPGTemplatePlayer player)
        {
            if (target != null) return;
            if (agroDistance < (transform.position - player.transform.position).SqrMagnitude)
            {
                target = player;
            }
        }

    }
}
