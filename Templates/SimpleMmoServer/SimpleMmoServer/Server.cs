using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.Examples.Instances;
 

namespace SimpleMmoServer
{
    /// <summary>
    /// Main custom server class
    /// Основной пользовательский класс сервера
    /// </summary>
    public class Server
    {
        /// <summary>
        /// An example of authorization implementation
        /// </summary>
        Authorization authorization = new Authorization();
        /// <summary>
        /// Registration example
        /// </summary>
        ExampleRegistration exampleRegistration = new ExampleRegistration();
        /// <summary>
        /// Server configuration
        /// </summary>
        Config config = new Config();
        /// <summary>
        /// The class that receives incoming messages
        /// Класс, который получает входящие сообщения
        /// </summary>
        InMessages inMessages = new InMessages();
        /// <summary>
        /// Logging class
        /// Класс логирования
        /// </summary>
        LogVisualizator logs = new LogVisualizator();
        /// <summary>
        /// Overriding network entities
        /// Переопределение сетевых сущностей
        /// </summary>
        NetEntityOverrider entityOverrider = new NetEntityOverrider();
        /// <summary>
        /// Whether to show debug messages
        /// Показывать ли дебаг сообщения
        /// </summary>
        internal static bool debugLog = true;

        public Server()
        {
            // Registering an incoming message handler
            // Регистрируем обработчик входящих сообщений
            config.incomingMessage = inMessages;
            GalaxyEvents.OnGalaxyInstanceCreate += OnGalaxyInstanceCreate; //Отлавливаем событие создания нового инстанса

            // It is important that the server name matches the name specified in the client
            // Важно что бы имя сервера совпадало с именем указанным в клиенте
            config.SERVER_NAME = "SimpleMmoServer";
            // Указываем рабочий порт
            // Specify the working port
            config.LISTEN_PORT = 30200;
            // enable auto control of the message sending buffer
            // включаем авто управление буфером отправки сообщений
            config.AUTO_FLUSH_SEND = true;
            config.NET_FRAME_RATE = 20;
            // We start the server
            // Запускаем сервер      
            GalaxyCore.Start(config);
                  
            //  GalaxyCore.instances.Create(new ExampleNetvisible());
            //  GalaxyCore.instances.Create(new SimpleMmoServer.RPGTemplate.Location());
        }

        /// <summary>
        /// This is an example of overriding the default instance implementation by custom code like
        /// Это пример переопределения стандартной реализации инстанса по пользовательскому коду типа
        /// </summary>
        /// <param name="type">custom instance type code</param>
        /// <param name="data">an array of bytes of additional information attached to the creation request</param>
        /// <param name="client">the client who sent the request</param>
        /// <returns>Return any inheritor of the Inctance class</returns>
        private Instance OnGalaxyInstanceCreate(byte type, byte[] data, Client client)
        {
            switch (type)
            {
                case 1:  
                    return new Examples.Instances.ExampleRoomPhys();                
                case 2:  
                    return new Examples.Instances.ExampleRoomMovers();
                case 3: 
                    return new Examples.Instances.ExampleOctoRoom();
                case 4:  
                    return new Examples.Instances.ExampleRoomPhys2();
                default:
                    // если не нужно ничего переопределять, то возвращяем null (будет использоваться стандартная комната)
                    return new Examples.Instances.ExampleEmtyInstance(); 
            }           
        }
    }
}
