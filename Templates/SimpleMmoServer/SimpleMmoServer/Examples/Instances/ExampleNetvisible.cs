using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Пример инстанса с включенной сетевой видимостью
    /// </summary>
    public class ExampleNetVisible : InstanceOpenWorldOctree
    {
        /// <summary>
        /// Создаем сохранялку для игровой карты
        /// </summary>
        public readonly MapSaver MapSaver = new MapSaver();

        public override void Start()
        {
            // не закрываться при выходе всех игроков
            AutoClose = false;
            // сетевая видимость по умолчанию в метрах
            VisibleDistance = 40;
            // имя инстанса
            Name = "Building example";
            // что грузим
            MapSaver.Load(this, "cs_mansion"); }

        public override void OutgoingClient(BaseClient clientConnection)
        {
            if (Clients.Count == 0) MapSaver.SaveInstance(this, "cs_mansion");
        }

        public override void Update()
        {
        }

        public override void InMessage(byte code, byte[] data, BaseClient clientConnection)
        {
        }
    }
}