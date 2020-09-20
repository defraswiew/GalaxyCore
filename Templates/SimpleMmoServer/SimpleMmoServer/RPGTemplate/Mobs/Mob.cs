using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.RPGTemplate
{
    public abstract class Mob : NetEntity
    {
        [GalaxyVar(1)]
        public int heal = 100;
        [GalaxyVar(2)]
        public byte state;
        [GalaxyVar(3)]
        public int maxHeal = 100;
        // какому спавнеру пренадлежит моб
        public MobSpawner spawner;
        // скорость движения
        public float moveSpeed = 0.1f;
        internal GalaxyVector3 movePoint;
        internal int agroDistance = 20;
        internal float attackDelay = 2;
        internal float attackDistanse = 2;
        internal int minDamage = 1;
        internal int maxDamage = 5;
        internal RPGTemplatePlayer target;
        internal int attackCount;
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
                transform.position = GalaxyVector3.Lerp(transform.position, movePoint, instance.Time.deltaTime * moveSpeed);
            } else
            {
                state = (byte)MobState.idle;
            }           
        }
        /// <summary>
        /// Атакуем или агримся
        /// </summary>
        public void Attack()
        {
            if(state>=20) return;
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
            if (sqrDis > agroDistance*2)
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
                transform.position = GalaxyVector3.Lerp(transform.position, target.transform.position, instance.Time.deltaTime * moveSpeed*2);              
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
                SendMessage((byte)MobState.attack, message.data,GalaxyDeliveryType.reliable);
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
