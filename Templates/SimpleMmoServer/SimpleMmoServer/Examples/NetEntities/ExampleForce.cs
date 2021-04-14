﻿using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.Instances;

namespace SimpleMmoServer.Examples.NetEntities
{
    class ExampleForce : NetEntity
    {
        public ExampleRoomPhys Room;
        private float _distance;

        public ExampleForce(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default,
            NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation,
            syncType)
        {
            PrefabName = "ExampleSphere";
        }


        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
            transform.SyncType = NetEntityAutoSync.position_and_rotation;
            Physics.Activate(new GalaxyCoreServer.Physics.ColliderSphere(0.5f));
            Physics.useGravity = false;
        }

        protected override void Update()
        {
            Physics.ApplyPhys();
            _distance = GalaxyVector3.Distance(Room.ForceTarget, transform.Position);
            if (_distance > 20)
            {
                Physics.AddForce((Room.ForceTarget - transform.Position) * 2f);
            }
            else
            {
                Physics.AddForce((transform.Position - Room.ForceTarget) * 6f);
            }
        }
    }
}