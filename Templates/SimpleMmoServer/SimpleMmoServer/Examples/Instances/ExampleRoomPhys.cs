using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples
{
    /// <summary>
    /// Комната демонстрирующая работу с физикой.
    /// </summary>
    public class ExampleRoomPhys : Instance
    {
        float timer; // 
        int boxCount; // текущее число боксов
        int boxMax = 100; // целевое число боксов


        public override void ClientExit(Client clientConnection)
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

            if (timer > 1)
            {               
                timer = 0;
                    ExamplePhysBox box = new ExamplePhysBox();
                    box.position = new GalaxyCoreCommon.GalaxyVector3();
                    box.position.y = 10;
                    box.rotation = new GalaxyCoreCommon.GalaxyQuaternion();
                    box.rotation.x = 4;
                    box.rotation.y = 10;
                    box.rotation.z = 20;
                    box.rotation.w = 0.5f;
                entities.CreateNetEntity(box);
                boxCount++;
            }
        }
    }
}
