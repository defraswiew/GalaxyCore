using System;
using System.Collections.Generic;
using System.Text;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;

namespace GalaxyTemplate
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
           
        }
    }
}
