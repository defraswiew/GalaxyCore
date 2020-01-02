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
            Console.WriteLine("Location ClietnExit");
        }

        public override void Close()
        {
            Console.WriteLine("Location Close");
        }

        public override void IncomingClient(ClientConnection clientConnection)
        {
            Console.WriteLine("Location IncomingClient");
        }

        public override void Start()
        {
            Console.WriteLine("Location Start");
        }

        public override void TossMessage(byte code, byte[] data, ClientConnection clientConnection)
        {
            Console.WriteLine("Location TossMessage");                   
        }
    }
}
