using GalaxyCoreServer;
using GalaxyCoreCommon;
using System;
using System.Collections.Generic;
using System.Text;
using SimpleMmoServer.Examples.Instances;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExamplePlayer : NetEntity
    {

        ExampleNavigationEntity pet;
      
        public ExamplePlayer(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "Player";
        }

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
         
        }

        public override void OnDestroy()
        {
            pet?.Destory();
        }

        public override void Start()
        {
            Log.Info("Player", "start");
            var navInstance = instance as ExampleNavigation;
            if (instance != null)
            {
                pet = new ExampleNavigationEntity(instance, this, navInstance.map);
                pet.transform.position = transform.position;
                pet.Init();
                Log.Info("Player", "Init pet");
            }
            
        }
       
     
        public override void Update()
        {            
     
        }
    }
}
