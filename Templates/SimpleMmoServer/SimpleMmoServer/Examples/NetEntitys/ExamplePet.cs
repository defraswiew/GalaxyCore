using System;
using System.Collections.Generic;
using System.Text;
using GalaxyCoreServer;
using GalaxyCoreCommon;


namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExamplePet : NetEntity
    {
        public NetEntity player;
        

        public ExamplePet(Instance instance, GalaxyVector3 position = null, GalaxyQuaternion rotation = null, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "Pet";          
        }
              

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
           
        }

        public override void OnDestroy()
        {
          
        }

        public override void Start()
        {
            transform.syncType = NetEntityAutoSync.position_and_rotation;           
        }

        public override void Update()
        {      
                if (GalaxyVector3.Distance(transform.position, player.transform.position) < 2) return;
                GalaxyVector3.LerpOptimize(transform.position, player.transform.position, instance.Time.deltaTime * 0.7f);
        }
    }
}
