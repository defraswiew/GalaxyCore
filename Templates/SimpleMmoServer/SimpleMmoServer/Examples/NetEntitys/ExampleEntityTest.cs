using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleEntityTest : NetEntity
    {
        public ExampleEntityTest(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
         
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
           
        }

        public override void OnDestroy()
        {
             
        }

        public override void Start()
        {
       
        }

        public override void Update()
        {
        
        }
    }
}
