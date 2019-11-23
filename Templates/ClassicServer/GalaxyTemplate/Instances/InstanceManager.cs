using GalaxyCoreServer.Api;
using GalaxyTemplateCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using GalaxyCoreServer;
using GalaxyTemplateCommon;
using System.Linq;

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
            room.maxClients = message.maxClients;
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

        internal void GetAllRoomsInfo(ClientConnection clientConnection)
        {
            if (instances.Count == 0)
            {
                //А смысл что то возвращять, если никаких инстансов сейчас не создано
                return;
            }
            RoomInfo info;
            MessageRoomInfo messageRoomInfo = new MessageRoomInfo();
            messageRoomInfo.rooms = new List<RoomInfo>();
            foreach (var item in instances.Keys)
            {
                info = new RoomInfo();
                info.clients = (uint)item.clients.Count;
                info.id = item.id;
                info.maxClients = item.maxClients;
                info.name = item.name;
                messageRoomInfo.rooms.Add(info);
            }
            clientConnection.SendMessage((byte)CommandType.roomGetList, messageRoomInfo, GalaxyCoreCommon.GalaxyDeliveryType.reliable);
        }

        /// <summary>
        /// Запрос на вход в комнату
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientConnection"></param>
        public void ClientEnter(int id,ClientConnection clientConnection)
        {
            Instance instanse = instances.Keys.Where(x => x.id == id).FirstOrDefault();
            if (instanse == null) return;
            instanse.AddClient(clientConnection);
        }

    }
}
