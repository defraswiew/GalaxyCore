using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleRandomMove : NetEntity
    {
        private GalaxyVector3 _target;
        private float _timer;
        private int _randTime;


        public ExampleRandomMove(Instance instance, GalaxyVector3 position = default,
            GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) :
            base(instance, position, rotation, syncType)
        {
            PrefabName = "Move";
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
        }

        protected override void Update()
        {
            _timer += Instance.Time.DeltaTime;
            if (_timer > _randTime)
            {
                _timer = 0;
                _randTime = GRand.NextInt(20, 45);
                _target.X = GRand.NextInt(-180, 180);
                _target.Z = GRand.NextInt(-180, 180);
            }

            transform.Position = GalaxyVector3.Lerp(transform.Position, _target, Instance.Time.DeltaTime * 0.03f);
        }
    }
}