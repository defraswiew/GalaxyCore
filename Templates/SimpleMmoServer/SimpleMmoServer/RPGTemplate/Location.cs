using GalaxyCoreCommon;
using GalaxyCoreServer; 
using System.Collections.Generic;

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример реализации большой локации с мобами
    /// </summary>
    public class Location : InstanceOpenWorldOctree
    {
        // список спавнеров мобов
        private readonly List<MobSpawner> _spawns = new List<MobSpawner>();

        public override void InMessage(byte code, byte[] data, BaseClient clientConnection)
        {
           
        }

        public override void Start()
        {
            // устанавливаем дистанцию видимости
            VisibleDistance = 50;
            // создаем спавнеры мобов
            _spawns.Add(new MobSpawner(new GalaxyVector3(65, 0, -35), this, "MobSlime", 15, 15, 10));
            _spawns.Add(new MobSpawner(new GalaxyVector3(100, 0, -50), this, "MobTurtle", 15, 15, 10));
            _spawns.Add(new MobSpawner(new GalaxyVector3(60, 0, -80), this, "MobSlimeBoss", 1, 10, 60));
             
            Name = "RPG Start Location";
            // не закрываем инстанс если в нем нет людей
            AutoClose = false;
        }

        public override void Update()
        {
           
        }

        // спавнер вызывает сам себя через инвойк
        public void SpawnerCall(MobSpawner spawner)
        {
            spawner.Respawn();
        }
    }
}
