using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.RPGTemplate
{
  public class MobSpawner
    {
        /// <summary>
        /// Точка на карте где расположен спавнер
        /// </summary>
        public GalaxyVector3 position;
        /// <summary>
        /// сколько должно быть мобов
        /// </summary>
        public int mobCount;
        /// <summary>
        /// Размер зоны по которой могут ходить мобы
        /// </summary>
        public int zoneSize = 10;
        /// <summary>
        /// Префаб мобов который нужно спавнить
        /// </summary>
        public string prefName;
        /// <summary>
        /// Период восстановления мобов
        /// </summary>
        public int respawnTime;
        /// <summary>
        /// Текущий инстанс
        /// </summary>
        private Instance instance;
        /// <summary>
        /// Мобы за которых ответственнен спавнер
        /// </summary>
        private List<Mob> mobs;
        
        /// <summary>
        /// Создать спавнер
        /// </summary>
        /// <param name="position">Координаты спавнера</param>
        /// <param name="instance">инстанс которому пренадлежит спавнер</param>
        /// <param name="prefName">Имя префаба который нужно спавнить</param>
        /// <param name="mobCount">сколько мобов </param>
        /// <param name="zoneSize">радиус зоны</param>
        /// <param name="respawnTime">время респавна моба</param>
        public MobSpawner(GalaxyVector3 position, Instance instance, string prefName, int mobCount = 10, int zoneSize = 10, int respawnTime = 30)
        {
            this.position = position;
            this.instance = instance;
            this.prefName = prefName;
            this.mobCount = mobCount;
            this.zoneSize = zoneSize;
            this.instance.InvokeRepeating("SpawnerCall", 1, respawnTime,this);
            mobs = new List<Mob>();
        }


        public void Respawn()
        {
            // если мобов достаточно, то выходим
            if (mobs.Count >= mobCount) return;

            Mob mob = null;
            switch (prefName)
            {
                case "MobSlime": mob = new MobSlime(instance); break;
                case "MobTurtle": mob = new MobTurtle(instance); break;
                case "MobSlimeBoss": mob = new MobSlimeBoss(instance); break;
                     
            }
            if (mob == null) return;
            mob.spawner = this;
            mob.transform.position = mob.RandomPoint();
            mob.Init();
            mobs.Add(mob);
        }

        public void Remove(Mob mob)
        {
            mobs.Remove(mob);
        }
    }
}
