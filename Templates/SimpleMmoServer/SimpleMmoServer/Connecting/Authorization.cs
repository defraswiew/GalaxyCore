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
    /// An example of authorization implementation
    /// Пример реализации авторизации
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Constructor, just for easy subscription
        /// Конструктор, просто для удобной подписки
        /// </summary>
        public Authorization()
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect; // подписываемся на событие
        }
        /// <summary>
        /// Here we will receive an authorization request
        /// Сюда нам придет запрос на авторизацию
        /// </summary>
        /// <param name="approvalConnection">Temporary unauthorized connection</param>
        /// <param name="data">Array of bytes, data sent along with the authorization request</param>
        private void OnGalaxyConnect(ApprovalConnection approvalConnection, byte[] data)
        {
            // converting the byte array into a readable message          
            // преобразовывем массив байт в читабельное сообщение          
            MessageAuth message = MessageAuth.Deserialize<MessageAuth>(data);
            // here we should carry out some kind of verification of the correctness of the login and password
            // But now we will not do this, and just allow the connection to everyone whose logins contain test
            // тут нам следовало бы провести какую либо проверку правильности логина и пароля
            // Но сейчас мы не станем этого делать, и просто разрешить соеденение всем в чьих логинах содержится test
            if (message.login.Contains("test"))
            {
                MessageApproval response = new MessageApproval(); //Создадим пакет который мы отправим клиенту вместе с разрешением коннекта
                response.name = message.login;
                int clientID = Tools.GetNewID(); // Получаем ид

                // ClientConnection connection;  
                // Create your own client implementation
                ExampleClient client = new ExampleClient();  
                client.name = message.login;
                // we return the data along with the permission, we will also get a working copy of the authorized connection
                // we also rivet our own client implementation, so that in the future, you can quickly get it from the connection
                // возвращяем данные вместе с разрешением, так же мы получим уже рабочий экземпляр авторизированного соеденения 
                // так же приклепляем собственную реализацию клиента, для того что бы в бущем, можно было её оперативно получить из коннекшена
                approvalConnection.Approve(response, clientID, client);
                Log.Debug("OnGalaxyConnect", "Client " + clientID + " connected");
            }
            else
            {
                 approvalConnection.Deny(1, "Нам не нравится ваш логин");
                Log.Debug("OnGalaxyConnect", "неудачный коннект");
            }
        }
    }
}
