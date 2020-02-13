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
        /// <param name="clientConnection">Экземпляр подключения клиента</param>
        public void IncomingMessage(byte code, byte[] data, ClientConnection clientConnection)
        {         
            // распределяем сообщения по заданному нами же коду, для удобства используем Enum    
            switch (code)
            {                

                //В остальных случаях отправляем сообщение в инстанс
                default:
                    if (clientConnection.instanse == null) return;
                    clientConnection.instanse.InMessage(code, data, clientConnection);
                    break;
            }

        }





    }
}
