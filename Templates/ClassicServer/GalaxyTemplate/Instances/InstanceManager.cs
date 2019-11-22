using GalaxyCoreServer.Api;
using GalaxyTemplateCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using GalaxyCoreServer;

namespace GalaxyTemplate.Instances
{
   internal class InstanceManager
    {
        /// <summary>
        /// Коллекция наших инстансов
        /// </summary>
        private ConcurrentBag<Instance> instances = new ConcurrentBag<Instance>();

        private int lastId = 0;

        /// <summary>
        /// Возвращяет новый id
        /// </summary>
        /// <returns></returns>
        private int GetNewID()
        {
            lastId++;
            return lastId;
        }


        /// <summary>
        /// Создать новую комнату
        /// </summary>
        /// <param name="data">данные от пользователя</param>
        /// <param name="clientConnection">Подключение пользователя</param>
        internal void CreateRoom(byte[] data, ClientConnection clientConnection)
        {
            MessageInstanceCreate message = MessageInstanceCreate.Deserialize<MessageInstanceCreate>(data);
            Room room = new Room(); // создаем новый экземпляр комнаты
            room.name = message.name;  // задаем имя комнаты
            room.id = GetNewID(); // задаем уникальный id комнаты, это id серверный, для сохранения в базу следует создать отдельную переменную
            instances.Add(room); // Добавляем инстанс в список
            room.AddClient(clientConnection); // добавляем клиента в созданный инстанс, 
        }


        

    }
}
