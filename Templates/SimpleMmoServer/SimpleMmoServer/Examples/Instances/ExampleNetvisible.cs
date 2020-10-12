using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate;
 
namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Пример инстанса с включенной сетевой видимостью
    /// </summary>
    public class ExampleNetvisible : InstanceOpenWorldOctree
    {
        /// <summary>
        /// Создаем сохранялку для игровой карты
        /// </summary>
        public MapSaver mapSaver = new MapSaver();

        public override void Start()
        {
            // не закрываться при выходе всех игроков
            autoClose = false;
            // сетевая видимость по умолчанию в метрах
            visibleDistance = 40;
            // имя инстанса
            name = "Building example";
            // что грузим
            mapSaver.Load(this, "cs_mansion");
        }

        public override void OutcomingClient(Client clientConnection)
        {
            // если вышел последний игрок, сохраняем изменение в карте
            if (clients.Count == 0) mapSaver.SaveInstance(this, "cs_mansion");
        }    
      
        public override void Update()
        {
           
        }
        public override void Close()
        {

        }

        public override void IncomingClient(Client clientConnection)
        {

        }



        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {

        }
    }
}
