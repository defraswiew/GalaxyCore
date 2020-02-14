using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.NetEntitys;
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
        float timer;
        int moverCount;
        int moverMax = 300;
        public override void ClientExit(ClientConnection clientConnection)
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
            SetFrameRate(30);
            Console.WriteLine("Location Start");
            /*
            for (int i = 0; i < 5; i++)
            {
                ExampleMonster monster = new ExampleMonster();
                monster.position.x = rnd.Next(5, 10);
                monster.position.z = rnd.Next(5, 10);
                entities.CreateNetEntity(monster);
            }
           */

        }

        public override void InMessage(byte code, byte[] data, ClientConnection clientConnection)
        {
            Console.WriteLine("Location TossMessage");                   
        }
        
        public override void Update()
        {         
            frameCount++;
            //   Log.Info("Location " + id,"frame " + frameCount + " Time " + Time.deltaTime);
            //  Thread.Sleep(rnd.Next(100,2000));   
            timer += Time.deltaTime;
            if (timer > 1)
            {
                if (moverCount > moverMax) return;
                timer = 0;
                ExampleRandomMove mover = new ExampleRandomMove();
                entities.CreateNetEntity(mover);
                moverCount++;
            }
        }
             
    }
}
