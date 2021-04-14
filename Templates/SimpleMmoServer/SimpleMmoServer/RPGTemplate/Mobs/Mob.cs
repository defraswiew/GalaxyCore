using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate;

namespace SimpleMmoServer.RPGTemplate.Mobs
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
        public int Heal = 100;

        /// <summary>
        /// текущее состояние
        /// </summary>
        [GalaxyVar(2)] 
        public byte State;

        /// <summary>
        /// Максимальное здоровье
        /// </summary>
        [GalaxyVar(3)] 
        public int MaxHeal = 100;

        /// <summary>
        /// Какому спавнеру пренадлежит моб
        /// </summary>
        public MobSpawner spawner;

        /// <summary>
        /// Множитель скорости движения моба
        /// </summary>
        public float MoveSpeed = 0.1f;

        /// <summary>
        /// Точка в которую движется моб
        /// </summary>
        internal GalaxyVector3 MovePoint;

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
        private float _lastAttack;


        public Mob(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default,
            NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation,
            syncType)
        {
        }

        /// <summary>
        /// Просим случайную точку в пределах текущего спавнера
        /// </summary>
        /// <returns></returns>
        public GalaxyVector3 RandomPoint()
        {
            if (spawner == null) return new GalaxyVector3();
            MovePoint.Y = spawner.Position.Y;
            MovePoint.X = GRand.NextInt((int) spawner.Position.X - spawner.ZoneSize,
                (int) spawner.Position.X + spawner.ZoneSize);
            MovePoint.Z = GRand.NextInt((int) spawner.Position.Z - spawner.ZoneSize,
                (int) spawner.Position.Z + spawner.ZoneSize);
            return MovePoint;
        }

        /// <summary>
        /// Двигаемся в заранее созданную случайную точку
        /// </summary>
        public void RandomMove()
        {
            if (State > 3) return;
            if ((transform.Position - MovePoint).SqrMagnitude > 2)
            {
                State = (byte) MobState.move;

                //      transform.position = GalaxyVector3.Lerp(transform.position, movePoint, instance.Time.deltaTime * moveSpeed);
                transform.Position = GalaxyVector3.Move(transform.Position, MovePoint, 1, Instance.Time.DeltaTime);
            }
            else
            {
                State = (byte) MobState.idle;
            }
        }

        /// <summary>
        /// Атакуем или агримся
        /// </summary>
        public void Attack()
        {
            if (State >= 20) return;
            // выходим если нет цели
            if (target == null) return;
            // выходим если цель мертва
            if (target.Heal <= 0)
            {
                target = null;
                State = 0;
                return;
            }

            float sqrDis = (target.transform.Position - transform.Position).SqrMagnitude;
            // выходим если цель ушла за предел агро зоны
            if (sqrDis > agroDistance * 2)
            {
                target = null;
                State = 0;
                return;
                attackCount = 0;
            }

            //Если игрок далеко, идем к нему
            if (sqrDis > attackDistanse)
            {
                State = (byte) MobState.follow;
                transform.Position = GalaxyVector3.Lerp(transform.Position, target.transform.Position,
                    Instance.Time.DeltaTime * MoveSpeed * 2);
                return;
            }

            // выдерживаем паузу между атаками
            if (Instance.Time.Time - _lastAttack < attackDelay) return;
            _lastAttack = Instance.Time.Time;
            // проверяем не пытался ли игрок подменить атаку
            attackCount++;
            if (!OnAttack()) return;
            State = (byte) MobState.attack;
            // рандомим дамаг
            int damage = GRand.NextInt(minDamage, maxDamage);
            BitGalaxy message = new BitGalaxy();
            message.WriteValue(damage);
            // отправляем сообщение о том что мы наносим кому то дамаг. 
            SendMessage((byte) MobState.attack, message.Data, GalaxyDeliveryType.reliable);
            // сообщяем игроку о уроне
            target.SetDamage(damage);
        }

        public abstract bool OnAttack();

        public void Death()
        {
            State = (byte) MobState.death;
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
            if (agroDistance < (transform.Position - player.transform.Position).SqrMagnitude)
            {
                target = player;
            }
        }
    }
}