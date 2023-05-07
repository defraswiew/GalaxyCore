using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.Instances;
#if GALAXY_DOUBLE
using scalar = System.Double;
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using scalar = System.Single;
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    class ExampleForce : NetEntity
    {
        public ExampleRoomPhys Room;
        private scalar _distance;

        public ExampleForce(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "ExampleSphere";
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
            Physics.Activate(new GalaxyCoreServer.Physics.ColliderSphere(0.5f));
            Physics.useGravity = false;
        }

        protected override void Update()
        {
            Physics.ApplyPhys();
            _distance = vector.Distance(Room.ForceTarget, transform.Position);
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