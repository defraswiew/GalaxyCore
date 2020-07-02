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
        public ExampleChangeOwner(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
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
            InvokeRepeating("Test", 10, 2, 5);
        }

        public void Test(int num)
        {
            Log.Info("Invoke", "test");
            //CancelInvoke("Test");
        }

        public override void Update()
        {
/*
            timer++;
            if (timer == 100) ChangeOwner();
            if (timer == 200)
            {
                ChangeOwner(oldOwner);
                timer = 0;
            }
            */
        }
    }
}
