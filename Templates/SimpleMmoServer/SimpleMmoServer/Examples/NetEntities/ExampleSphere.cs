using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleSphere : NetEntity
    {
        public ExampleSphere(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            TransformAuthority = NetEntityTransformAuthority.Server;
            PrefabName = "ExampleSphere";
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
            // Create a new collider
            // создаем новый коллайдер
            var collider = PhysicsShape.Sphere(0.5f);
            // activate physics
            // активируем физику
            Physics.Create(new PhysicsBodyOptions { Shape = collider, Mass = 1f, Restitution = 1f });
            // set the weight of the object in kg
            // устанавливае вес объекта в кг


            // specify the synchronization method
            // указываем способ синхронизации

            Physics.CollisionEntered += OnCollisionEnter;
        }

        private void OnCollisionEnter(PhysicsCollision collision)
        {
            Log.Info("OnCollisionEnter", collision.OtherEntity?.PrefabName ?? collision.OtherBodyId.ToString());
        }

        protected override void Update()
        {
            // Physics snapshots are applied automatically by the owning Instance.
        }
    }
}
