using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using GalaxyTemplate.Clients;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System;
using System.Collections.Generic;
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
        /// Вызывается когда кто то вышел из комнаты
        /// </summary>
        /// <param name="clientConnection"></param>
        public override void ClietnExit(ClientConnection clientConnection)
        {
            if (Server.debugLog) {
                Client client = Server.clientManager.GetClientByConnection(clientConnection);
                Console.WriteLine("Клиент ID:" + client.id + " покинул комнату ID:" + this.id);
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
                        TransformGO(message);
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
            SendMessageToAll((byte)CommandType.goInstantiate,message,GalaxyDeliveryType.reliable);
        }

        void TransformGO(MessageTransform message)
        {
            SendMessageToAll((byte)CommandType.goTransform, message, GalaxyDeliveryType.unreliableNewest);
        }

        #endregion



    }
}
