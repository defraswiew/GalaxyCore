using GalaxyCoreCommon;
using GalaxyCoreCommon.NetEntity;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using GalaxyTemplate.Clients;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyTemplate.Instances
{
    /// <summary>
    /// Пример реализации комнаты
    /// </summary>
   public class Room: Instance
    {
        /// <summary>
        /// Удалить ли комнату когда она пуста
        /// </summary>
        public bool delWhenEmpty = false;

        public Dictionary<int,NetGO> netObjects = new Dictionary<int, NetGO>();

        private static int lastId = 0;

        /// <summary>
        /// Возвращяем новый ID
        /// </summary>
        /// <returns></returns>
        private int GetNewID()
        {
            lastId++;
            return lastId;
        }

        public Room()
        {
          
        }

        #region Клиенты 
        /// <summary>
        /// Вызывается когда игрок присоеденился к комнате
        /// </summary>
        /// <param name="clientConnection"></param>
        public override void IncomingClient(ClientConnection clientConnection)
        {
            //Для начала оповестим нашего пользователя об успешном входе в комнату
            SendRoomEnterSusses(clientConnection);       
        }
        /// <summary>
        /// Отправка текущего состояния мира
        /// </summary>
        private void SendWorld(ClientConnection clientConnection)
        {
            NetGO[] netGOs = netObjects.Values.ToArray(); // берем все наши сетевые объекты
            if (netGOs.Length == 0) return; //раз нет сетевых объектов то выходим
            MessageWorldSync message = new MessageWorldSync();
            message.netObjects = new List<MessageInstantiate>();
            foreach (var item in netGOs)
            {
                MessageInstantiate instantiate = new MessageInstantiate();
                instantiate.name = item.name;
                instantiate.netID = item.netID;
                instantiate.owner = item.owner;
                instantiate.position = item.position;
                instantiate.rotation = item.rotation;
                message.netObjects.Add(instantiate);
            }
            clientConnection.SendMessage((byte)CommandType.worldSync, message, GalaxyDeliveryType.reliable);
        }

        /// <summary>
        /// Вызывается когда кто то вышел из комнаты
        /// </summary>
        /// <param name="clientConnection"></param>
        public override void ClietnExit(ClientConnection clientConnection)
        {
            if (Server.debugLog) {
                Client client = Server.clientManager.GetClientByConnection(clientConnection);
                Console.WriteLine("Клиент ID:" + client.id + " покинул комнату ID:" + this.id);               
                int[] clientGoList = netObjects.Where(x => x.Value.owner == client.id).Select(x => x.Key).ToArray(); // собираем ид всех объектов клиента
                foreach (var item in clientGoList)
                {
                    DestroyGO(item);
                }
            }
            if (clients.Count == 0 && delWhenEmpty)
            {
                //удаляем комнату если в ней не осталось игроков, и если это предусмотрено настройками комнаты
                Server.instanceManager.RemoveRoom(this);
            }
        }
        /// <summary>
        /// Отправляем игроку который хотел войти в комнату ответ с разрешением входа.
        /// </summary>
        /// <param name="clientConnection">Коннект клиента</param>
        private void SendRoomEnterSusses(ClientConnection clientConnection)
        {
            MessageRoomEnter message = new MessageRoomEnter(); // создаем новое сообщение
            message.id = this.id; // текущий ид инстанса
            message.name = this.name; //текущее имя инстанса
            clientConnection.SendMessage((byte)CommandType.roomEnter, message, GalaxyDeliveryType.reliable); // отправляем сообщение
            if (Server.debugLog)
            {
                Client client = Server.clientManager.GetClientByConnection(clientConnection);
                Console.WriteLine("Клиент ID:" + client.id + " присоеденился к комнате ID:" + this.id);
            }
        }
        #endregion
        public override void Start()
        {

        }
        public override void Close()
        {
            
        }
     


        #region Внутренний сепаратор
        /// <summary>
        /// Сюда попадают проброшенные сообщения
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="clientConnection"></param>
        public override void TossMessage(byte code, byte[] data, ClientConnection clientConnection)
        {
            switch ((CommandType)code)
            {
                case CommandType.goInstantiate:
                    {  
                    MessageInstantiate message = MessageInstantiate.Deserialize <MessageInstantiate>(data);
                    InstantiateGO(message, clientConnection);
                    }
                    break;
                case CommandType.goTransform:
                    {
                        MessageTransform message = MessageTransform.Deserialize<MessageTransform>(data);
                        TransformGO(message, clientConnection);
                    }
                    break;
                case CommandType.goDestroy:
                    {
                        MessageDestroyGO message = MessageDestroyGO.Deserialize<MessageDestroyGO>(data);
                        DestroyGO(message, clientConnection);
                    }
                    break;
                case CommandType.worldSync:
                    {
                        SendWorld(clientConnection);
                    }
                    break;
                case CommandType.goMessage:
                    {
                        SendMessageToAllExcept(clientConnection, (byte)CommandType.goMessage, data, GalaxyDeliveryType.reliable);
                    }
                    break;
            }
        }
        #endregion


        #region Работа с объектами
        /// <summary>
        /// Обработчик создания новых объектов
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clientConnection"></param>
        void InstantiateGO(MessageInstantiate message, ClientConnection clientConnection)
        {           
            Client client = Server.clientManager.GetClientByConnection(clientConnection);
            NetGO netGO = new NetGO();
            netGO.name = message.name;
            netGO.netID = GetNewID();           
            netGO.owner = client.id;
            netGO.position = message.position;
            netGO.rotation = message.rotation;            
            message.netID = netGO.netID;
            message.owner = client.id;
            netObjects.Add(netGO.netID, netGO);
            SendMessageToAll((byte)CommandType.goInstantiate,message,GalaxyDeliveryType.reliable);
            Console.WriteLine("Был создан NetGO id:" + netGO.netID);
        }

        /// <summary>
        /// Объект хочет двигаться
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clientConnection"></param>
        void TransformGO(MessageTransform message, ClientConnection clientConnection)
        {          
            Client client = Server.clientManager.GetClientByConnection(clientConnection);
            NetGO go;
            if (!netObjects.TryGetValue(message.netID, out go)) return; // объект не найден
            if (go.owner != client.id) return; // объект не пренадлежит игроку
            SendMessageToAll((byte)CommandType.goTransform, message, GalaxyDeliveryType.unreliableNewest);          
        }

        /// <summary>
        /// Запрос на удаление объекта
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clientConnection"></param>
        void DestroyGO(MessageDestroyGO message, ClientConnection clientConnection)
        {       
            Client client = Server.clientManager.GetClientByConnection(clientConnection);
            NetGO go;        
            if (!netObjects.TryGetValue(message.netID, out go)) return; // объект не найден
            if (go.owner != client.id) return; // объект не пренадлежит игроку
            netObjects.Remove(message.netID,out go);
            SendMessageToAll((byte)CommandType.goDestroy, message, GalaxyDeliveryType.reliable);
            Console.WriteLine("Был удален NetGO id:" + message.netID);
        }

        /// <summary>
        /// Удаление объекта по инициативе сервера
        /// </summary>
        /// <param name="netID"></param>
        void DestroyGO(int netID)
        {           
            NetGO go;
            if (!netObjects.TryGetValue(netID, out go)) return; // объект не найден        
            netObjects.Remove(netID, out go);
            MessageDestroyGO message = new MessageDestroyGO();
            message.netID = netID;
            SendMessageToAll((byte)CommandType.goDestroy, message, GalaxyDeliveryType.reliable);
            Console.WriteLine("Был удален NetGO id:" + message.netID);
        }

     
        #endregion



    }
}
