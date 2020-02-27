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
        float timer; // 
        int boxCount; // текущее число боксов
        int boxMax = 500; // целевое число боксов
          

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
            physics.Activate(); // активизуем физику в рамках данной комнаты 

        }

        public override void Update()
        {
            if (boxCount < boxMax) BoxSpawn();
        }


        private void BoxSpawn()
        {
            timer += Time.deltaTime;

            if (timer > 0.1f)
            {               
                timer = 0;
                    Examples.NetEntitys.ExamplePhysBox box = new Examples.NetEntitys.ExamplePhysBox(this, new GalaxyVector3(0, 10, 0),new GalaxyQuaternion(4, 10, 20, 0.5f));
                    box.Init();
                   boxCount++;
            }
        }
    }
}
