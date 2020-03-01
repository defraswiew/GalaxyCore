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

        public ExamplePlayer(Instance instance, GalaxyVector3 position = null, GalaxyQuaternion rotation = null, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
        }

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
         
        }

        public override void OnDestroy()
        {
            instance.entities.RemoveNetEntity(pet);  
        }

        public override void Start()
        {
            Log.Info("ExamplePlayer", "Start");
            transform.autoApplyRemoteTransform = true;
            pet = new ExamplePet(instance, transform.position + new GalaxyVector3(1, 0, 1));
            pet.player = this;
            pet.Init();
            physics.Activate(new GalaxyCoreServer.Physics.ColliderBox(new GalaxyVector3(1, 1, 1)));          
        }
       

        public override void Update()
        {
            physics.SetPhys();
        }
    }
}
