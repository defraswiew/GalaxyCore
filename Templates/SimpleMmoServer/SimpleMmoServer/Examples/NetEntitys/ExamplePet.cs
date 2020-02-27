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
        public ExamplePet()
        {
            name = "Pet";
        }

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
           
        }

        public override void OnDestroy()
        {
          
        }

        public override void Start()
        {
            syncType = NetEntityAutoSync.position_and_rotation;
        }

        public override void Update()
        {
            if (GalaxyVector3.Distance(position, player.position) < 2) return;
            GalaxyVector3.LerpOptimize(position, player.position, instance.Time.deltaTime * 0.7f);
        }
    }
}
