﻿using GalaxyCoreServer;
using GalaxyCoreCommon;
#if GALAXY_DOUBLE
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExamplePet : NetEntity
    {
        public NetEntity Player;

        public ExamplePet(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "Pet";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient client)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnRemotePosition(GalaxyVector3 remotePosition)
        {
             
        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {
             
        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {
             
        }

        protected override void Start()
        {
           
        }

        protected override void Update()
        {
            if (vector.Distance(transform.Position, Player.transform.Position) < 2) return;
            transform.Position = vector.Lerp(transform.Position, Player.transform.Position,
                Instance.Time.DeltaTime * 0.7f);
        }
    }
}