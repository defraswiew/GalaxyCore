using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System;
using System.Collections.Generic;
using System.Text;
using Unigine;
using UnigineApp.GalaxyNetwork;

namespace UnigineApp
{
    public class UnigineGalaxyNetwork:Plugin
    {
        /// <summary>
        /// client configuration
        /// Конфигурации клиента
        /// </summary>
        GalaxyCoreLib.Config config = new GalaxyCoreLib.Config();

        #region UI
        UILogin uILogin = new UILogin();
        UIInstances uiInstances = new UIInstances();
        UIStatistics uIStatistics = new UIStatistics();
        UIChat uIChat = new UIChat();
        #endregion


        public override bool Init()
        {
            App.SetBackgroundUpdate(true);
            // Server ip address
            // Ip адрес сервера
            config.serverIp = "127.0.0.1";
            // Server port
            // Порт сервера
            config.serverPort = 30200;
            // Client name, must match the server name
            // Имя клиента, должно соответствовать имени сервера
            config.app_name = "SimpleMmoServer";
            // The network frame is the default rate. (automatically regulated by the server)
            // Сетевой фрейм рейт по уполчанию. (автоматически регулируется сервером)
            config.FrameRate = 25; 

            // initialize the network core
            // инициализируем сетевое ядро       
            GalaxyClientCore.Initialize(config);           
            GalaxyClientCore.unityCalls.Awake();

            // subscribe to the event of a successful connection
            // подписываемся на событие удачного коннекта
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;

            // subscribe to the instance(room) enter event
            // подписываемся на событие входа в инстанс
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            // Show login window
            // Вызываем окно входа
            uILogin.Drow();
            return true;
        }

        /// <summary>
        /// Called when entering a room
        /// </summary>
        /// <param name="info"></param>
        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
           uiInstances.Close();
           GalaxyApi.instances.SyncInstance();
        }
        /// <summary>
        /// Called upon successful connection to the server
        /// Срабатывает при успешном подключении к серверу
        /// </summary>
        /// <param name="message"></param>
        private void OnGalaxyConnect(byte[] message)
        {
           uILogin.Close();
           uIStatistics.Drow();
           uiInstances.Drow();
           uIChat.Drow();
        }

        public override void Update()
        {
            GalaxyClientCore.unityCalls.Update(Game.IFps);  
        }


        public override bool Shutdown()
        {
            GalaxyApi.connection.Disconnect();
            GalaxyClientCore.unityCalls.OnApplicationQuit();
            return true;
        }
       
    }
}
