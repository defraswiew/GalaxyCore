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
        public ExamplePhysBox(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default,
            NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation,
            syncType)
        {
            //Задаем имя префаба соответствующего данному объекту
            PrefabName = "ExampleBox";
        }

        protected override void Start()
        {
            // создаем рабочий коллайдер. Без коллайдера объект не может учавствовать в физическом представлении
            ColliderBox collider = new ColliderBox(new GalaxyVector3(0.4f, 0.4f, 0.4f));
            Physics.Activate(collider); // активируем физику     
            Physics.Mass = 1f; // устанавливае вес объекта в кг
            transform.SyncType = NetEntityAutoSync.position_and_rotation; // указываем способ синхронизации 
        }


        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
        }

        protected override void Update()
        {
            // применяем физические расчеты к объекту
            Physics.ApplyPhys();
        }


        protected override void OnDestroy()
        {
        }
    }
}