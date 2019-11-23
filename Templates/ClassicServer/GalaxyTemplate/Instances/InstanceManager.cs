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
        private ConcurrentDictionary<Instance,Object> instances = new ConcurrentDictionary<Instance, Object>();

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
            instances.TryAdd(room,null); // Добавляем инстанс в список
            if (Server.debugLog) Console.WriteLine("Клиент ID:" + Server.clientManager.GetClientByConnection(clientConnection).id + " Создал комнату ID:"+ room.id);
            room.AddClient(clientConnection); // добавляем клиента в созданный инстанс,            
        }

        /// <summary>
        /// Удалить инстанс
        /// </summary>
        /// <param name="instance"></param>
        internal void RemoveRoom(Instance instance)
        {
            if (instance.clients.Count > 0)
            {
                Console.WriteLine("Нельзя удалить комнату, сначала надо всех выгнать");
                return;
            }
            Object obj;
            instances.TryRemove(instance,out obj);
            if (Server.debugLog) Console.WriteLine("Комната ID:" + instance.id + " была удалена");
        }

        

    }
}
