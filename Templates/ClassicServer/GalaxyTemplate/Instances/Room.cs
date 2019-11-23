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
             
        public Room()
        {
          
        }
        /// <summary>
        /// Вызывается когда кто то вышел из комнаты
        /// </summary>
        /// <param name="clientConnection"></param>
        public override void ClietnExit(ClientConnection clientConnection)
        {
           if (Server.debugLog) { 
            Client client = Server.clientManager.GetClientByConnection(clientConnection);
            Console.WriteLine("Клиент ID:"+ client.id + " покинул комнату ID:"+this.id);
            }
            if (clients.Count == 0 && delWhenEmpty)
            {
                //удаляем комнату если в ней не осталось игроков, и если это предусмотрено настройками комнаты
                Server.instanceManager.RemoveRoom(this); 
            }
        }

        public override void Close()
        {
            
        }
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
        /// Отправляем игроку который хотел войти в комнату ответ с разрешением входа.
        /// </summary>
        /// <param name="clientConnection">Коннект клиента</param>
        private void SendRoomEnterSusses(ClientConnection clientConnection)
        {
            MessageRoomEnter message = new MessageRoomEnter(); // создаем новое сообщение
            message.id = this.id; // текущий ид инстанса
            message.name = this.name; //текущее имя инстанса
            clientConnection.SendMessage((byte)CommandType.roomEnter, message, GalaxyDeliveryType.reliable); // отправляем сообщение
            if (Server.debugLog) { 
            Client client = Server.clientManager.GetClientByConnection(clientConnection);
            Console.WriteLine("Клиент ID:"+ client.id + " присоеденился к комнате ID:"+this.id);
            }
        }

    }
}
