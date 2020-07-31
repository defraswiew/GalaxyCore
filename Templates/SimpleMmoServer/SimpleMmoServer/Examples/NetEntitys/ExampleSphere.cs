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
            // Create a new collider
            // создаем новый коллайдер 
            ColliderSphere collider = new ColliderSphere(0.5f);
            // activate physics
            // активируем физику
            physics.Activate(collider);
            // set the weight of the object in kg
            // устанавливае вес объекта в кг
            physics.mass = 1f;
            physics.material.bounciness = 2;           
            // specify the synchronization method
            // указываем способ синхронизации      
            transform.syncType = NetEntityAutoSync.position_and_rotation;      
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
