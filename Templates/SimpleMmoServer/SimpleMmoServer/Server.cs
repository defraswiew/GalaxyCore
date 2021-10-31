using System;
using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.Connecting;
using SimpleMmoServer.NetEntities;

namespace SimpleMmoServer
{
    /// <summary>
    /// Main custom server class
    /// Основной пользовательский класс сервера
    /// </summary>
    public class Server
    {
        /* To help me quickly fix bugs, plug in the "Sentry" package to NuGet. Uncomment these lines, and indicate the name of the project. Thanks.
        private IDisposable _debugger = SentrySdk.Init(o =>
        {
            o.Dsn = "https://3f0d0ab79e9b497db744524ed6af612b@o997840.ingest.sentry.io/6042409";
            o.Debug = true;
            o.TracesSampleRate = 1.0;
            o.Release = "you project name";
            o.AttachStacktrace = true;
            o.MaxBreadcrumbs = 20;
        });
        */
        /// <summary>
        /// An example of authorization implementation
        /// </summary>
        private Authorization _authorization = new Authorization();

        /// <summary>
        /// Registration example
        /// </summary>
        private ExampleRegistration _exampleRegistration = new ExampleRegistration();

        /// <summary>
        /// Server configuration
        /// </summary>
        private Config _config = new Config();

        /// <summary>
        /// The class that receives incoming messages
        /// Класс, который получает входящие сообщения
        /// </summary>
        private InMessages _inMessages = new InMessages();

        /// <summary>
        /// Logging class
        /// Класс логирования
        /// </summary>
        private LogVisualizator _logs = new LogVisualizator();

        /// <summary>
        /// Overriding network entities
        /// Переопределение сетевых сущностей
        /// </summary>
        private NetEntityOverrider _entityOverrider = new NetEntityOverrider();

        /// <summary>
        /// Whether to show debug messages
        /// Показывать ли дебаг сообщения
        /// </summary>
        internal static bool debugLog = true;

        public Server()
        {
            // Registering an incoming message handler
            // Регистрируем обработчик входящих сообщений
            _config.IncomingMessage = _inMessages;
            GalaxyEvents.OnGalaxyInstanceCreate +=
                OnGalaxyInstanceCreate; //Отлавливаем событие создания нового инстанса

            // It is important that the server name matches the name specified in the client
            // Важно что бы имя сервера совпадало с именем указанным в клиенте
            _config.ServerName = "SimpleMmoServer";
            // Указываем рабочий порт
            // Specify the working port
            _config.ListenPort = 30200;
            // enable auto control of the message sending buffer
            // включаем авто управление буфером отправки сообщений
            _config.AutoFlushSend = true;
            _config.NetFrameRate = 20;
            _config.DebugMode = true;
            // We start the server
            // Запускаем сервер      
            //GalaxyCore.Instances.Create(new RPGTemplate.Location());
            
            GalaxyCore.Start(_config);
        }
        
        

        /// <summary>
        /// This is an example of overriding the default instance implementation by custom code like
        /// Это пример переопределения стандартной реализации инстанса по пользовательскому коду типа
        /// </summary>
        /// <param name="type">custom instance type code</param>
        /// <param name="data">an array of bytes of additional information attached to the creation request</param>
        /// <param name="client">the client who sent the request</param>
        /// <returns>Return any inheritor of the Instance class</returns>
        private Instance OnGalaxyInstanceCreate(byte type, byte[] data, BaseClient client)
        {
            switch (type)
            {
                case 1:
                    return new Examples.Instances.ExampleRoomPhys();
                case 2:
                    return new Examples.Instances.ExampleRoomMovers();
                case 4:
                    return new Examples.Instances.ExampleRoomPhys2();
                case 5:
                    return new Examples.Instances.ExampleNavigation();
                default:
                    // если не нужно ничего переопределять, то возвращяем null (будет использоваться стандартная комната)
                    return new Examples.Instances.ExampleEmptyInstance();
            }
        }
    }
}