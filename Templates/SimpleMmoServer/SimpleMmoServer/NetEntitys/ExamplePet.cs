using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.NetEntitys
{
    public class ExamplePet : NetEntity
    {
        private NetEntity target;
        public override void InMessage(byte externalCode, byte[] data, ClientConnection client)
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
            if (target == null)
            {
                FindTarget();
                return;
            }
        }

        private void FindTarget()
        {
           
        }

    }
}
