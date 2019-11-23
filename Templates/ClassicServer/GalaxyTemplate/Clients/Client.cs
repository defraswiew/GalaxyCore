using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplate.Clients
{
    /// <summary>
    /// Вариант реализации класса клиента
    /// </summary>
   public class Client
    {
        /// <summary>
        /// Соеденение клиента
        /// </summary>
       public ClientConnection clientConnection;
        /// <summary>
        /// Ид клиента
        /// </summary>
        public int id;

        public Client(ClientConnection clientConnection, int id)
        {
            this.clientConnection = clientConnection;
            this.id = id;
        }

    }
}
