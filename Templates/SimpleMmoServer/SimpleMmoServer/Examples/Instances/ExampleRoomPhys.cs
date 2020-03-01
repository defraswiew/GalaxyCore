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
        int bodyMax = 200; // целевое число боксов       

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
            if (bodyCount < bodyMax) BoxSpawn();
        }


        private void BoxSpawn()
        {
            timer += Time.deltaTime;

            if (timer > 0.1f)
            {               
                timer = 0;
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
        }
    }
}
