using System;
using System.Collections.Generic;
using System.Text;
using GalaxyCoreServer;
using GalaxyCoreCommon;
using GalaxyCoreServer.Api;
using GalaxyTemplate.Connecting;

namespace GalaxyTemplate
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

        InMessages inMessages = new InMessages();

        public Server()
        {
            config.incomingMessage = inMessages; // Регистрируем обработчик входящих сообщений
            //Задаем имя сервера
            //Важно что бы имя сервера совпадало с именем указанным в клиенте
            config.SERVER_NAME = "ClassicTemplate";
            config.LISTEN_PORT = 30200; // Указываем рабочий порт
            GalaxyCore.Start(config); // Запускаем сервер
        }

    }
}
