using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
   public class ExampleEmtyInstance : Instance
    {
        public override void Close()
        {
          
        }

        public override void IncomingClient(Client clientConnection)
        {
           
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
        
        }

        public override void OutcomingClient(Client clientConnection)
        {
          
        }

        public override void Start()
        {
            Log.Info("ExampleEmtyInstance", "instance id:" + id);// выводим в консоль тип комнаты
            SetFrameRate(30); // устанавливаем подходящий врейм рейт
        }

        public override void Update()
        {
         
        }
    }
}
