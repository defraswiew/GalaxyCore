using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer
{
   public class Client
    {
        public ClientConnection connection;
        public int id;
        public void Init(ClientConnection clientConnection, int id)
        {
            connection = clientConnection;
            this.id = id;
        }

    }
}
