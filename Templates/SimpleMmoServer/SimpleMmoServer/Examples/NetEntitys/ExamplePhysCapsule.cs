using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExamplePhysCapsule : NetEntity
    {
        public ExamplePhysCapsule(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "ExampleCapsule";
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
           
        }

        public override void OnDestroy()
        {
          
        }

        public override void Start()
        {
            ColliderCapsule collider = new ColliderCapsule(2,0.5f);
            physics.Activate(collider); // активируем физику     
            physics.mass = 1f; // устанавливае вес объекта в кг
            transform.syncType = NetEntityAutoSync.position_and_rotation; // указываем способ синхронизации 
        }

        public override void Update()
        {
            physics.ApplyPhys();
        }
    }
}
