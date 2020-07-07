using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleSphere : NetEntity
    {
        public ExampleSphere(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "ExampleSphere";
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
           
        }

        public override void OnDestroy()
        {
          
        }

        public override void Start()
        {
            ColliderSphere collider = new ColliderSphere(0.5f);
            physics.Activate(collider); // активируем физику     
            physics.mass = 1f; // устанавливае вес объекта в кг
            transform.syncType = NetEntityAutoSync.position_and_rotation; // указываем способ синхронизации           
            physics.OnCollisionEnter += OnCollisionEnter;
        }

        private void OnCollisionEnter(Collision collision)
        {          
            Log.Info("OnCollisionEnter", collision.tag);
        }

        public override void Update()
        {
            physics.ApplyPhys();
        }
    }
}
