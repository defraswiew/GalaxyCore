using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExamplePlayer : NetEntity
    {
        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
         
        }

        public override void OnDestroy()
        {
           
        }

        public override void Start()
        {
            Log.Info("ExamplePlayer", "Start");
            autoApplyRemoteTransform = true;
            syncType = NetEntityAutoSync.position_and_rotation;          
        }

        public override void Update()
        {
         
        }
    }
}
