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
            Log.Debug("OnGalaxyRegistration", data.Length.ToString());
            // преобразовывем массив байт в читабельное сообщение      
            // converting the byte array into a readable message
            var message = BaseMessage.Deserialize<MessageAuth>(data);
            // here it is worth doing a check in the database, for the presence of the same account, and similar checks, (instead of if (true))
            // тут стоит делать проверку в базу, на наличие такого же аккаунта, и тому подобные проверки, (вместо  if (true))
            if (true)
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