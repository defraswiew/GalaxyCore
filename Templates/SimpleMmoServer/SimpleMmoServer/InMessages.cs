using GalaxyCoreServer;
 

namespace SimpleMmoServer
{
    /// <summary>
    /// Main receiver of incoming packets
    /// Главный приемщик входящих пакетов
    /// Сюда попадают собщения отправленные через  GalaxyApi.send.SendMessageToServer
    /// </summary>
    public class InMessages : IIncomingMessage
    {
        /// <summary>
        /// Implementing incoming message processing
        /// </summary>
        /// <param name="code">The message code that we attached from the client side</param>
        /// <param name="data">Array of bytes (message)</param>
        /// <param name="client">Client instance</param>
        public void IncomingMessage(byte code, byte[] data, Client client)
        {
            // we distribute messages according to the code we specified, for convenience we use Enum   
            switch (code)
            {
                case 211:
                    Log.Debug("IncomingMessage", "Encrypt test");
                    break;
                //In other cases, we send a message to the instance
                default:
                    if (client.instanse == null) return;
                    client.instanse.InMessage(code, data, client);
                    break;
            }

        }
    }
}
