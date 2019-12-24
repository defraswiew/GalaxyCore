using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer
{
    public class Location : Instance
    {
        public override void ClietnExit(ClientConnection clientConnection)
        {
         //   throw new NotImplementedException();
        }

        public override void Close()
        {
         //   throw new NotImplementedException();
        }

        public override void IncomingClient(ClientConnection clientConnection)
        {
          //  throw new NotImplementedException();
        }

        public override void TossMessage(byte code, byte[] data, ClientConnection clientConnection)
        {
          //  throw new NotImplementedException();
        }
    }
}
