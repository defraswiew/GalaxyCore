using GalaxyCoreCommon;
using GalaxyCoreServer;
using System.Collections.Generic;
using SimpleMmoServer.RPGTemplate.Mobs;

namespace SimpleMmoServer.RPGTemplate
{
    public class MobSpawner
    {
        /// <summary>
        /// Точка на карте где расположен спавнер
        /// </summary>
        public GalaxyVector3 Position;

        /// <summary>
        /// сколько должно быть мобов
        /// </summary>
        public int MobCount;

        /// <summary>
        /// Размер зоны по которой могут ходить мобы
        /// </summary>
        public int ZoneSize = 10;

        /// <summary>
        /// Префаб мобов который нужно спавнить
        /// </summary>
        public string PrefName;

        /// <summary>
        /// Период восстановления мобов
        /// </summary>
        public int RespawnTime;

        /// <summary>
        /// Текущий инстанс
        /// </summary>
        private Instance _instance;

        /// <summary>
        /// Мобы за которых ответственнен спавнер
        /// </summary>
        private List<Mob> _mobs;

        /// <summary>
        /// Создать спавнер
        /// </summary>
        /// <param name="position">Координаты спавнера</param>
        /// <param name="instance">инстанс которому пренадлежит спавнер</param>
        /// <param name="prefName">Имя префаба который нужно спавнить</param>
        /// <param name="mobCount">сколько мобов </param>
        /// <param name="zoneSize">радиус зоны</param>
        /// <param name="respawnTime">время респавна моба</param>
        public MobSpawner(GalaxyVector3 position, Instance instance, string prefName, int mobCount = 10,
            int zoneSize = 10, int respawnTime = 30)
        {
            Position = position;
            _instance = instance;
            PrefName = prefName;
            MobCount = mobCount;
            ZoneSize = zoneSize;
            _instance.InvokeRepeating("SpawnerCall", 1, respawnTime, this);
            _mobs = new List<Mob>();
        }


        public void Respawn()
        {
            // если мобов достаточно, то выходим
            if (_mobs.Count >= MobCount) return;

            Mob mob = null;
            switch (PrefName)
            {
                case "MobSlime":
                    mob = new MobSlime(_instance);
                    break;
                case "MobTurtle":
                    mob = new MobTurtle(_instance);
                    break;
                case "MobSlimeBoss":
                    mob = new MobSlimeBoss(_instance);
                    break;
            }

            if (mob == null) return;
            mob.spawner = this;
            mob.transform.Position = mob.RandomPoint();
            mob.Init();
            _mobs.Add(mob);
        }

        public void Remove(Mob mob)
        {
            _mobs.Remove(mob);
        }
    }
}