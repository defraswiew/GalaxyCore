using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoCommon.Messages;

namespace SimpleMmoServer.Connecting
{
    /// <summary>
    /// Класс пример регистрации
    /// </summary>
    public class ExampleRegistration
    {
        public ExampleRegistration()
        {
            // подписываемся на событие регистрации
            GalaxyEvents.OnGalaxyRegistration += OnGalaxyRegistration;
        }

        private void OnGalaxyRegistration(ApprovalConnection approvalConnection, byte[] data)
        {
            Log.Debug("OnGalaxyRegistration", (data?.Length ?? 0).ToString());
            // преобразовывем массив байт в читабельное сообщение
            // converting the byte array into a readable message
            if (!GalaxyMessage.TryDeserialize<MessageAuth>(data, out var message) || string.IsNullOrWhiteSpace(message.Login))
            {
                approvalConnection.Deny(1, "Invalid authentication payload");
                return;
            }
            // Replace this demo condition with account lookup and credential validation.
            // Замените условие проверкой аккаунта и учётных данных в базе.
            if (!string.IsNullOrWhiteSpace(message.Password))
            {
                // Let's create a package that we will send to the client along with the connection permission
                // Создадим пакет который мы отправим клиенту вместе с разрешением коннекта
                var response = new MessageApproval
                {
                    Name = message.Login
                };
                // Instead of Tools.GetNewID, you need to substitute the real ID from the database, obtained after registration.
                // Вместо Tools.GetNewID нужно подставить реальный ид из базы, полученный после регистрации.
                int clientID = Tools.GetNewID();


                var client = new ExampleClient
                {
                    Name = message.Login
                };

                approvalConnection.Approve(response, clientID, client);
            }
            else
            {
                approvalConnection.Deny(1, "Deny");
            }
        }
    }
}
