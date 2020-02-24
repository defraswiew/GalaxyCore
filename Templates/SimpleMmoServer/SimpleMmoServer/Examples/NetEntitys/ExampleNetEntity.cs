using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoCommon;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleNetEntity : NetEntity
    {
        public override void Start()
        {
          
        }
        public override void InMessage(byte externalCode, byte[] data, Client client)
        {   
           
            switch ((NetEntityCommand)externalCode)
            {
                case NetEntityCommand.syncTransform:                  
                    // проверяем может ли игрок двигать этот объект
                    if (owner != client.id) return;    
                    
                    // если все в порядке то отправляем в логику трансформа
                    SyncTransform(data, client);
                    break;
            
            }
             
        }

        private void SyncTransform(byte[] data, Client client)
        {           
            // читаем сообщение
            MessageTransform message = BaseMessage.Deserialize<MessageTransform>(data);           
            // если позиция не пуста значит обновляем
            if (message.position != null) position = message.position;         
            // если поворот не пуст то обновляем
            if (message.rotation != null) rotation = message.rotation;

            // сообщяем всем экземплярам объекта что нужно обновить трансформ
      //  Log.Info("SyncTransform", "SendMessageExcept");
            SendMessageExcept(client, (byte)NetEntityCommand.syncTransform, data, GalaxyDeliveryType.unreliableNewest);               
        }
        public override void OnDestroy()
        {
            
        }

        public override void Update()
        {
         
        }
    }
}
