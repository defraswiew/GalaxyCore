﻿using GalaxyCoreCommon;
using GalaxyCoreServer;
#if GALAXY_DOUBLE
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleFollow : NetEntity
    {
        public NetEntity Target;
        private bool _isRed;

        public ExampleFollow(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default,
            NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation,
            syncType)
        {
            PrefabName = "ExampleFollow";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
            if (Target == null) return;
            transform.Position =
                vector.Lerp(transform.Position, Target.transform.Position, Instance.Time.DeltaTime);
            var distance = vector.Distance(transform.Position, Target.transform.Position);
            if (distance < 2)
            {
                if (_isRed) return;
                _isRed = true;
                SendMessage(2, new byte[1] {1});
            }
            else
            {
                if (!_isRed) return;
                _isRed = false;
                SendMessage(2, new byte[1] {0});
            }
        }
    }
}