using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer
{
   public class ExampleRegistration
    {
        public ExampleRegistration()
        {
            GalaxyEvents.OnGalaxyRegistration += OnGalaxyRegistration;
        }

        private void OnGalaxyRegistration(ApprovalConnection approvalConnection, byte[] data)
        {
            Log.Debug("OnGalaxyRegistration", data.Length.ToString());
            MessageAuth message = MessageAuth.Deserialize<MessageAuth>(data); //преобразовывем массив байт в читабельное сообщение            
            //тут нам следовало бы провести какую либо проверку правильности логина и пароля
            //тут стоит делать проверку в базу, на наличие такого же аккаунта, и тому подобные проверки, (вместо  if (true))
            if (true)
            {
                MessageApproval response = new MessageApproval(); //Создадим пакет который мы отправим клиенту вместе с разрешением коннекта
                response.name = message.login;
                int clientID = Tools.GetNewID(); // Вместо Tools.GetNewID нужно подставить реальный ид из базы, полученный после регистрации.

                // ClientConnection connection; // Раз мы решили авторизировать клиента, то следует создать уже постоянное соеденение
                ExampleClient client = new ExampleClient(); // Создаем собственную реализацию клиента
                client.name = message.login;
                // возвращяем данные вместе с разрешением, так же мы получим уже рабочий экземпляр авторизированного соеденения 
                // так же приклепляем собственную реализацию клиента, для того что бы в бущем, можно было её оперативно получить из коннекшена
                approvalConnection.Approve(response, clientID, client);

            }
            else
            {
                //Ну а раз test в логине не нашлось, то соеденение не разрешаем
                //Тут нам нужно отправить два оргумента
                // первый это код ошибки, вы можете указать его любым, от вернется клиенту для дайнейшей обработки
                // второй это читабельное сообщение об ошибки, впрочим это не обязательно
                approvalConnection.Deny(1, "Нам не нравится ваш логин");
            }
        }
    }
}
