using GalaxyCoreServer;
using SimpleMmoServer.Connecting;
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
        /// Показывать ли дебаг сообщения
        /// </summary>
        internal static bool debugLog = true;

        public Server()
        {
            config.incomingMessage = inMessages; // Регистрируем обработчик входящих сообщений
            //Задаем имя сервера
            //Важно что бы имя сервера совпадало с именем указанным в клиенте
            config.SERVER_NAME = "SimpleMmoServer";
            config.LISTEN_PORT = 30200; // Указываем рабочий порт
            GalaxyCore.Start(config); // Запускаем сервер
        }

    }
}
