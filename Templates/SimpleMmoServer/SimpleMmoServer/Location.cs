using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SimpleMmoServer
{
    public class Location : Instance
    {
        int frameCount = 0;
        Random rnd = new Random();
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
            SetFrameRate(2);
            Console.WriteLine("Location Start");
        }

        public override void TossMessage(byte code, byte[] data, ClientConnection clientConnection)
        {
            Console.WriteLine("Location TossMessage");                   
        }

        public override void Update(float deltaTime)
        {
            Console.WriteLine("start");
            frameCount++;
            Log.Info("Location " + id,"frame " + frameCount);
            Thread.Sleep(rnd.Next(100,2000));
            Console.WriteLine("end");
        }
             
    }
}
