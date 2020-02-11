using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.Connecting;
using SimpleMmoServer.NetEntitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer
{
   public class Server
    {
        /// <summary>
        /// Пример реализации авторизации
        /// </summary>
        Authorization authorization = new Authorization();
        /// <summary>
        /// Конфигурация сервера
        /// </summary>
        Config config = new Config();
        /// <summary>
        /// Класс получающий входящие сообщения
        /// </summary>
        InMessages inMessages = new InMessages();
        /// <summary>
        /// Класс логирования
        /// </summary>
        LogVisualizator logs = new LogVisualizator();
        /// <summary>
        /// Показывать ли дебаг сообщения
        /// </summary>
        internal static bool debugLog = true;

        public Server()
        {
            config.incomingMessage = inMessages; // Регистрируем обработчик входящих сообщений
            GalaxyEvents.OnGalaxyInstanceCreate += OnGalaxyInstanceCreate; //Отлавливаем событие создания нового инстанса
            GalaxyEvents.OnNetEntityInstantiate += OnNetEntityInstantiate; //Отлавливаем событие создания сетевой сущности для пераназначения логики
            //Задаем имя сервера
            //Важно что бы имя сервера совпадало с именем указанным в клиенте
            config.SERVER_NAME = "SimpleMmoServer";
            config.LISTEN_PORT = 30200; // Указываем рабочий порт
            GalaxyCore.Start(config); // Запускаем сервер
        }

        private NetEntity OnNetEntityInstantiate(string name, byte[] data, ClientConnection clientConnection)
        {
            ExampleNetEntity netEntity = new ExampleNetEntity();
            return netEntity;
        }

        /// <summary>
        /// Это пример переопределения стандартной реализации инстанса
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="clientConnection"></param>
        /// <returns></returns>
        private Instance OnGalaxyInstanceCreate(byte type, byte[] data, ClientConnection clientConnection)
        {
            Location location = new Location();
            Console.WriteLine("Переопределение Инстанса в локацию");
            return location;
        }
    }
}
