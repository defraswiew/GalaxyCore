﻿using GalaxyCoreCommon;
using GalaxyCoreServer;
#if GALAXY_DOUBLE
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleRandomMove : NetEntity
    {
        private vector _target;
        private GalaxyQuaternion _targetRotation;
        private float _timer;
        private int _randTime;


        public ExampleRandomMove(Instance instance, GalaxyVector3 position = default,
            GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) :
            base(instance, position, rotation, syncType)
        {
            PrefabName = "Move";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient client)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
            transform.SyncType = NetEntityAutoSync.position_and_rotation;
        }

        protected override void Update()
        {
            _timer += Instance.Time.DeltaTime;
            if (_timer > _randTime)
            {
                _timer = 0;
                _randTime = GRand.NextInt(20, 45);
                _target.X = GRand.NextInt(-180, 180);
                _target.Y = GRand.NextInt(-180, 180);
                _target.Z = GRand.NextInt(-180, 180);
                
                _targetRotation.X = GRand.NextInt(-100, 100) * 0.01f;
                _targetRotation.Y = GRand.NextInt(-100, 100) * 0.01f;
                _targetRotation.Z = GRand.NextInt(-100, 100) * 0.01f;
                _targetRotation.W = GRand.NextInt(-100, 100) * 0.01f;
                _targetRotation = _targetRotation.Normalize();
            }

            transform.Position = vector.Lerp(transform.Position, _target, Instance.Time.DeltaTime * 0.03f);
            transform.Rotation =
                GalaxyQuaternion.Lerp(transform.Rotation, _targetRotation, Instance.Time.DeltaTime * 0.03f);
            
        }
    }
}