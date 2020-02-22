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
        int moverMax = 200;
        public override void ClientExit(Client client)
        {                     
            Console.WriteLine("Location ClietnExit");
        }

        public override void Close()
        {
            Console.WriteLine("Location Close");
        }
        
        public override void IncomingClient(Client client)
        {
          
          
            Console.WriteLine("Location IncomingClient");
        }

        public override void Start()
        {            
            SetFrameRate(20);
         //   physics.Activate();
          //  physics.multithread = true;
            Console.WriteLine("Location Start");
            /*
            for (int i = 0; i < 10; i++)
            {
                ExampleMonster monster = new ExampleMonster();
                monster.position.x = rnd.Next(5, 10);
                monster.position.z = rnd.Next(5, 10);
                entities.CreateNetEntity(monster);
            }
          */

        }
        public override void InMessage(byte code, byte[] data, Client client)
        {
            Console.WriteLine("Location TossMessage");                   
        }
        
        public override void Update()
        {
         //   SendMessageToAll(99, data, GalaxyCoreCommon.GalaxyDeliveryType.unreliableNewest);
            frameCount++;
            //  SendMessageToAll(99, data, GalaxyCoreCommon.GalaxyDeliveryType.reliable);
            //   Log.Info("Location " + id,"frame " + frameCount + " Time " + Time.deltaTime);
            //  Thread.Sleep(rnd.Next(100,2000));   
            //   return;
            
            timer += Time.deltaTime;

            if (timer > 0.5f)
            {
                if (moverCount > moverMax) return;
                timer = 0;
              
                    ExampleRandomMove monster = new ExampleRandomMove();
                    monster.position.y = 1;
                    monster.rotation = new GalaxyCoreCommon.GalaxyQuaternion();
                    monster.rotation.x = 0;
                    monster.rotation.y = 0;
                    monster.rotation.z = 0;
                    monster.rotation.w = 0.1f;
                    entities.CreateNetEntity(monster);
                moverCount++;
            }
                          
                
              
            }
             }
            
        }
             
 
 
