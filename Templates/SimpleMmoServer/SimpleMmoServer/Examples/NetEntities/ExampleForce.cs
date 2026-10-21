using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;
using SimpleMmoServer.Examples.Instances;
#if GALAXY_DOUBLE
using Scalar = System.Double;
using Vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using Scalar = System.Single;
using Vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    class ExampleForce : NetEntity
    {
        public ExampleRoomPhys Room;
        private Scalar _distance;

        public ExampleForce(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            TransformAuthority = NetEntityTransformAuthority.Server;
            PrefabName = "ExampleSphere";
        }


        public override void InMessage(byte externalCode, byte[] data, BaseClient client)
        {
        }

        protected override void OnDestroy(byte[] customData)
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
            Physics.Create(new PhysicsBodyOptions { Shape = PhysicsShape.Sphere(0.5f), AffectedByGravity = false });

        }

        protected override void Update()
        {
            // Physics snapshots are applied automatically by the owning Instance.
            _distance = Vector.Distance(Room.ForceTarget, transform.Position);
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
