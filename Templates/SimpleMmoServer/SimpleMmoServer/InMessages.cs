using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer
{
    /// <summary>
    /// Главный приемщик входящих пакетов
    /// </summary>
    public class InMessages : IIncomingMessage // обазятельное наследование интерфейса, т.к именно через него в данный класс входят новые сообщения
    {
        /// <summary>
        /// Реализация обработки входящих сообщений
        /// </summary>
        /// <param name="code">Код сообщения который мы приложили со стороны клиента</param>
        /// <param name="data">Массив байт (сообщение)</param>
        /// <param name="client">Экземпляр клиента</param>
        public void IncomingMessage(byte code, byte[] data, Client client)
        {
          
            // распределяем сообщения по заданному нами же коду, для удобства используем Enum    
            switch (code)
            {
                case 211:
                    Log.Debug("IncomingMessage", "Encrypt test");
                    break;
                //В остальных случаях отправляем сообщение в инстанс
                default:
                    if (client.instanse == null) return;
                    client.instanse.InMessage(code, data, client);
                    break;
            }

        }





    }
}
