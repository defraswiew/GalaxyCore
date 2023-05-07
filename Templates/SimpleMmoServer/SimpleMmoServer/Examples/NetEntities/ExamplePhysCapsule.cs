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
            PrefabName = "ExampleCapsule";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
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
            ColliderCapsule collider = new ColliderCapsule(2, 0.5f);
            Physics.Activate(collider); // активируем физику     
            Physics.Mass = 1f; // устанавливае вес объекта в кг
        }

        protected override void Update()
        {
            Physics.ApplyPhys();
        }
    }
}