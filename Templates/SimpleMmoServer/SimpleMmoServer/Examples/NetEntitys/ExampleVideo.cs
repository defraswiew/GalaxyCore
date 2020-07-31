using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleVideo : NetEntity
    {
        ExampleFollow example;
        public ExampleVideo(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
           
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
            SendMessageExcept(clientSender, externalCode, data);
        }

        public override void OnDestroy()
        {
            example.Destory();
        }

        public override void Start()
        {
            example = new ExampleFollow(instance);
            example.target = this;
            example.Init();
        }

        public override void Update()
        {
          
        }
    }
}
