using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Комната демонстрирующая работу с физикой.
    /// </summary>
    public class ExampleRoomPhys : Instance
    {
        float timer = -3; // 
        int bodyCount; // текущее число боксов
        int bodyMax = 100; // целевое число боксов       
        int bodyForseCount;
        int bodyForseMax = 200;
        public GalaxyVector3 forseTarget = new GalaxyVector3(10, 10, 10);
        int frameCount;


        public override void OutcomingClient(Client clientConnection)
        {
          
        }

        public override void Close()
        {
           
        }

        public override void IncomingClient(Client clientConnection)
        {
         
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
          
        }

        public override void Start()
        {
            Log.Info("ExampleRoomPhys", "instance id:"+id);// выводим в консоль тип комнаты
            SetFrameRate(20); // устанавливаем подходящий врейм рейт
                              // physics.Activate("phys/ExamplePhys.phys"); // активизуем физику в рамках данной комнаты      
            physics.Activate();
        }

        public override void Update()
        {
            timer += Time.deltaTime;

            if (timer > 0.1f)
            {
                timer = 0;
                if (bodyCount < bodyMax) BoxSpawn();
                if (bodyForseCount < bodyForseMax) BodyForseSpawn();
            }

            frameCount++;
            if (frameCount % 300 == 0)
            {
                forseTarget = new GalaxyVector3(GRand.NextInt(0, 100), GRand.NextInt(0, 100), GRand.NextInt(0, 100));
            }
        }


        private void BoxSpawn()
        {          

                if(bodyCount % 2 == 0) {

                    Examples.NetEntitys.ExampleSphere sphere = new Examples.NetEntitys.ExampleSphere(this, new GalaxyVector3(0.36f, 15, 17.5f), new GalaxyQuaternion(4, 10, 20, 0.5f));
                    sphere.Init();
                    bodyCount++;
                    return;
                }
                    Examples.NetEntitys.ExamplePhysBox box = new Examples.NetEntitys.ExamplePhysBox(this, new GalaxyVector3(0.36f, 15, 17.5f),new GalaxyQuaternion(4, 10, 20, 0.5f));
                    box.Init();
                   bodyCount++;
        }

        private void BodyForseSpawn()
        {
            Examples.NetEntitys.ExampleForse entity = new Examples.NetEntitys.ExampleForse(this, new GalaxyVector3(GRand.NextInt(5, 10), GRand.NextInt(5, 10), GRand.NextInt(5, 10)), new GalaxyQuaternion(0, 0, 0, 0));
            entity.room = this;
            entity.Init();
            bodyForseCount++;
        }
    }
}
