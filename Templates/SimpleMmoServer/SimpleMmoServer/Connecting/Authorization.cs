using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer
{
    /// <summary>
    /// Пример реализации авторизации
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Конструктор, просто для удобной подписки
        /// </summary>
        public Authorization()
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect; // подписываемся на событие
        }
        /// <summary>
        /// Сюда нам придет запрос на авторизацию
        /// </summary>
        /// <param name="approvalConnection">Временное не авторизированное соеденение</param>
        /// <param name="data">Массив байт, данные присланные вместе с запросом авторизации</param>
        private void OnGalaxyConnect(ApprovalConnection approvalConnection, byte[] data)
        {
            
            MessageAuth message = MessageAuth.Deserialize<MessageAuth>(data); //преобразовывем массив байт в читабельное сообщение            
            //тут нам следовало бы провести какую либо проверку правильности логина и пароля
            //Но сейчас мы не станем этого делать, и просто разрешить соеденение всем в чьих логинах содержится test
            if (message.login.Contains("test"))
            {
                MessageApproval response = new MessageApproval(); //Создадим пакет который мы отправим клиенту вместе с разрешением коннекта
                response.name = message.login;
                int clientID = Tools.GetNewID(); // Получаем ид
             
               // ClientConnection connection; // Раз мы решили авторизировать клиента, то следует создать уже постоянное соеденение
                ExampleClient client = new ExampleClient(); // Создаем собственную реализацию клиента
                client.name = message.login;
                // возвращяем данные вместе с разрешением, так же мы получим уже рабочий экземпляр авторизированного соеденения 
                // так же приклепляем собственную реализацию клиента, для того что бы в бущем, можно было её оперативно получить из коннекшена
                approvalConnection.Approve(response, clientID, client);
                Log.Debug("OnGalaxyConnect", "Client " + clientID + " connected");
            }
            else
            {
                //Ну а раз test в логине не нашлось, то соеденение не разрешаем
                //Тут нам нужно отправить два оргумента
                // первый это код ошибки, вы можете указать его любым, от вернется клиенту для дайнейшей обработки
                // второй это читабельное сообщение об ошибки, впрочим это не обязательно
                approvalConnection.Deny(1, "Нам не нравится ваш логин");
                Log.Debug("OnGalaxyConnect", "неудачный коннект");
            }
        }
    }
}
