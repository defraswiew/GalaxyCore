using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExamplePhysCapsule : NetEntity
    {
        public ExamplePhysCapsule(Instance instance, GalaxyVector3 position = default,
            GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) :
            base(instance, position, rotation, syncType)
        {
            PrefabName = "ExampleCapsule";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
            ColliderCapsule collider = new ColliderCapsule(2, 0.5f);
            Physics.Activate(collider); // активируем физику     
            Physics.Mass = 1f; // устанавливае вес объекта в кг
            transform.SyncType = NetEntityAutoSync.position_and_rotation; // указываем способ синхронизации 
        }

        protected override void Update()
        {
            Physics.ApplyPhys();
        }
    }
}