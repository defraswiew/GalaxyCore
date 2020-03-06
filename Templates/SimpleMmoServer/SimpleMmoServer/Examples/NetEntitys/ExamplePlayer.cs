using GalaxyCoreServer;
using GalaxyCoreCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExamplePlayer : NetEntity
    {

        ExamplePet pet;
        public int test;
        bool old_res = false;
        public ExamplePlayer(Instance instance, GalaxyVector3 position = null, GalaxyQuaternion rotation = null, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "Player";
        }

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
         
        }

        public override void OnDestroy()
        {
            // instance.entities.RemoveNetEntity(pet);  
          
        }

        public override void Start()
        {
            Log.Info("ExamplePlayer", "Start");
            transform.autoApplyRemoteTransform = true;
            transform.syncType = NetEntityAutoSync.position_and_rotation;
            //  pet = new ExamplePet(instance, transform.position + new GalaxyVector3(1, 0, 1));
            // pet.player = this;
            //   pet.Init();
            physics.Activate(new GalaxyCoreServer.Physics.ColliderSphere(1));
            test = netID;
        }
       
        public void SendRayCastResult(bool res)
        {
            if (old_res == res) return;
            old_res = res;
            byte code = 200;
            if (res) code = 201;
            SendMessage(code, new byte[0], GalaxyDeliveryType.reliable);
        }

        public override void Update()
        {            
          physics.SetPhys();             
        }
    }
}
