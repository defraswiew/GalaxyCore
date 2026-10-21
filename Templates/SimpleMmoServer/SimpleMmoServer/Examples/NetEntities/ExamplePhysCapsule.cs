using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExamplePhysCapsule : NetEntity
    {
        public ExamplePhysCapsule(Instance instance, GalaxyVector3 position = default,
            GalaxyQuaternion rotation = default) :
            base(instance, position, rotation)
        {
            TransformAuthority = NetEntityTransformAuthority.Server;
            PrefabName = "ExampleCapsule";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
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
            var collider = PhysicsShape.Capsule(0.5f, 2f);
            Physics.Create(new PhysicsBodyOptions { Shape = collider, Mass = 1f }); // активируем физику

        }

        protected override void Update()
        {
            // Physics snapshots are applied automatically by the owning Instance.
        }
    }
}
