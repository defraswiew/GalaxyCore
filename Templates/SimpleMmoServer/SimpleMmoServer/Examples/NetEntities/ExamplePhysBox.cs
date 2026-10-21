using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;

namespace SimpleMmoServer.Examples.NetEntities
{
    /// <summary>
    /// Сетевой объект показывающий пример работы бокс коллайдера
    /// </summary>
    public class ExamplePhysBox : NetEntity
    {
        public ExamplePhysBox(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            //Задаем имя префаба соответствующего данному объекту
            TransformAuthority = NetEntityTransformAuthority.Server;
            PrefabName = "ExampleBox";
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
            // создаем рабочий коллайдер. Без коллайдера объект не может учавствовать в физическом представлении
            var collider = PhysicsShape.Box(new GalaxyVector3(0.4f, 0.4f, 0.4f));
            Physics.Create(new PhysicsBodyOptions { Shape = collider, Mass = 1f }); // активируем физику

        }


        public override void InMessage(byte externalCode, byte[] data, BaseClient client)
        {
        }

        protected override void Update()
        {
            // применяем физические расчеты к объекту
            // Physics snapshots are applied automatically by the owning Instance.
        }


        protected override void OnDestroy(byte[] customData)
        {
        }
    }
}
