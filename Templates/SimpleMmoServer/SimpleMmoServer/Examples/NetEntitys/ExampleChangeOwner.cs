using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
   public class ExampleChangeOwner : NetEntity
    {
        int timer = 0;
        public Client oldOwner; 
        public ExampleChangeOwner(Instance instance, GalaxyVector3 position = null, GalaxyQuaternion rotation = null, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
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
            oldOwner = instance.GetClientById(ownerClientId);
        }

        public override void Update()
        {

            timer++;
            if (timer == 100) ChangeOwner();
            if (timer == 200)
            {
                ChangeOwner(oldOwner);
                timer = 0;
            }
        }
    }
}
