using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
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
        public Room()
        {
           
        }

        public override void Close()
        {
            
        }

        public override void IncomingClient(ClientConnection clientConnection)
        {
            //Для начала оповестим нашего пользователя об успешном входе в комнату
            SendRoomEnterSusses(clientConnection);
        }


        private void SendRoomEnterSusses(ClientConnection clientConnection)
        {
            MessageRoomEnter message = new MessageRoomEnter(); // создаем новое сообщение
            message.id = this.id; // текущий ид инстанса
            message.name = this.name; //текущее имя инстанса
            clientConnection.SendMessage((byte)CommandType.roomEnter, message, GalaxyDeliveryType.reliable); // отправляем сообщение
        }

    }
}
