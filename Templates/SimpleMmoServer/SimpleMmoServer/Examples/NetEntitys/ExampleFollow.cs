using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleFollow : NetEntity
    {
        public NetEntity target;
        bool isRed = false;
        public ExampleFollow(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "ExampleFollow";
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
            if (target == null) return;
            transform.position = GalaxyVector3.Lerp(transform.position, target.transform.position, instance.Time.deltaTime);
            float distanse = GalaxyVector3.Distance(transform.position, target.transform.position);
            if (distanse < 2)
            {
                if (isRed) return;
                isRed = true;
                SendMessage(2, new byte[1] { 1 });
            } else
            {
                if (!isRed) return;
                isRed = false;
                SendMessage(2, new byte[1] { 0 });
            }
        }
    }
}
