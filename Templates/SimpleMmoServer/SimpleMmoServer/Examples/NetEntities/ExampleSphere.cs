using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;
 
namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleSphere : NetEntity
    {
        public ExampleSphere(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "ExampleSphere";
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
            // Create a new collider
            // создаем новый коллайдер 
            ColliderSphere collider = new ColliderSphere(0.5f);
            // activate physics
            // активируем физику
            Physics.Activate(collider);
            // set the weight of the object in kg
            // устанавливае вес объекта в кг
            Physics.Mass = 1f;
            Physics.Material.bounciness = 2;
            // specify the synchronization method
            // указываем способ синхронизации      
           
            Physics.OnCollisionEnter += OnCollisionEnter;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Log.Info("OnCollisionEnter", collision.Tag);
        }

        protected override void Update()
        {
            Physics.ApplyPhys();
        }
    }
}