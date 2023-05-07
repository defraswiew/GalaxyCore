using GalaxyCoreCommon;
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
            GalaxyQuaternion rotation = default) :
            base(instance, position, rotation)
        {
            PrefabName = "Move";
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
            _timer += Instance.Time.DeltaTime;
            if (_timer > _randTime)
            {
                _timer = 0;
                _randTime = GRand.NextInt(20, 45);
                _target.X = GRand.NextInt(-50, 50);
                _target.Y = GRand.NextInt(-50, 50);
                _target.Z = GRand.NextInt(-50, 50);
                
                _targetRotation.X = GRand.NextInt(-10, 100) * 0.01f;
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