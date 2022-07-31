using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using Unigine;
using UnigineApp.source.GalaxyNetwork.UI;

namespace UnigineApp.source.GalaxyNetwork
{
    public class UnigineGalaxyNetwork:Plugin
    {
        /// <summary>
        /// client configuration
        /// Конфигурации клиента
        /// </summary>
        readonly GalaxyCoreLib.Config _config = new GalaxyCoreLib.Config();

        #region UI
        private readonly UILogin _uILogin = new UILogin();
        private readonly UIInstances _uiInstances = new UIInstances();
        private readonly UIStatistics _uIStatistics = new UIStatistics();
        private readonly UIChat _uIChat = new UIChat();
        #endregion


        public override bool Init()
        {
            App.SetBackgroundUpdate(true);
            // Server ip address
            // Ip адрес сервера
            _config.ServerIp = "127.0.0.1";
            // Server port
            // Порт сервера
            _config.ServerPort = 30200;
            // Client name, must match the server name
            // Имя клиента, должно соответствовать имени сервера
            _config.AppName = "SimpleMmoServer";
            // The network frame is the default rate. (automatically regulated by the server)
            // Сетевой фрейм рейт по уполчанию. (автоматически регулируется сервером)
            _config.FrameRate = 25; 

            // initialize the network core
            // инициализируем сетевое ядро       
            GalaxyClientCore.Initialize(_config);           
            GalaxyClientCore.EngineCalls.Awake();

            // subscribe to the event of a successful connection
            // подписываемся на событие удачного коннекта
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;

            // subscribe to the instance(room) enter event
            // подписываемся на событие входа в инстанс
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            // Show login window
            // Вызываем окно входа
            _uILogin.Show();
            return true;
        }

        /// <summary>
        /// Called when entering a room
        /// </summary>
        /// <param name="info"></param>
        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
           _uiInstances.Close();
           GalaxyApi.Instances.SyncInstance();
        }
        /// <summary>
        /// Called upon successful connection to the server
        /// Срабатывает при успешном подключении к серверу
        /// </summary>
        /// <param name="message"></param>
        private void OnGalaxyConnect(byte[] message)
        {
           _uILogin.Close();
           _uIStatistics.Show();
           _uiInstances.Show();
           _uIChat.Show();
        }

        public override void Update()
        {
            GalaxyClientCore.EngineCalls.Update(Game.IFps);  
        }


        public override bool Shutdown()
        {
            GalaxyApi.Connection.Disconnect();
            GalaxyClientCore.EngineCalls.OnApplicationQuit();
            return true;
        }
    }
}