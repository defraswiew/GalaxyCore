using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    class ExampleForse:NetEntity
    {
        public ExampleRoomPhys room;
        float distanse;

        public ExampleForse(Instance instance, GalaxyVector3 position = null, GalaxyQuaternion rotation = null, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "ExampleSphere";
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
            physics.Activate(new GalaxyCoreServer.Physics.ColliderSphere(0.5f));
            physics.useGravity = false;
        }

        public override void Update()
        {
            physics.ApplyPhys();
            distanse = GalaxyVector3.Distance(room.forseTarget, transform.position);
            if (distanse > 20)
            {
                physics.AddForсe((room.forseTarget - transform.position) * 2f);
            } else
            {
                physics.AddForсe((transform.position - room.forseTarget) * 6f);
            }


        }
    }
}
