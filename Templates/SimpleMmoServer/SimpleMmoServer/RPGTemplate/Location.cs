using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.RPGTemplate
{
    public class Location : InstanceOpenWorldOctree
    {
        // список спавнеров мобов
        List<MobSpawner> spawners = new List<MobSpawner>();
        public override void Close()
        {
         
        }

        public override void IncomingClient(Client clientConnection)
        {
          
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
           
        }

        public override void OutcomingClient(Client clientConnection)
        {
            
        }

        public override void Start()
        {
            // устанавливаем дистанцию видимости
            visibleDistance = 50;
            spawners.Add(new MobSpawner(new GalaxyVector3(65, 0, -35), this, "MobSlime", 15, 15, 10));
            spawners.Add(new MobSpawner(new GalaxyVector3(100, 0, -50), this, "MobTurtle", 15, 15, 10));
            spawners.Add(new MobSpawner(new GalaxyVector3(60, 0, -80), this, "MobSlimeBoss", 1, 10, 60));
             
            name = "RPG Start Location";
            // не закрываем инстанс если в нем нет людей
            autoClose = false;
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
