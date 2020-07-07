using GalaxyCoreCommon;
using GalaxyCoreServer;
 
using SimpleMmoServer.Examples.NetEntitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleOctoRoom : InstanceOpenWorld
    {
        int tick = 0;
        Random random = new Random();
        
        public ExampleOctoRoom()
        {
          
        }

        public override void Close()
        {
          
        }

        public override void IncomingClient(Client clientConnection)
        {
            foreach (var item in entities.list)
            {
            item.ChangeOwner(clientConnection);
            }    
        }   

        public override void Start()
        {          
            Invoke("TestInvoke", 5, 10,"привет");
            InvokeRepeating("TestInvokeRepeating", 5, 1, "Я повторяюсь раз в секунду","и еще текста");
        }

        public void TestInvoke(int num, string name)
        {
            Log.Info("TestInvoke", "Complete " + num +  "  " +name);
        }

        public void TestInvokeRepeating(string message, string test)
        {
            Log.Info("TestInvokeRepeating", "Complete " + name + " " + test);
        }

         
        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {

        }

        public override void OutcomingClient(Client clientConnection)
        {

        }
        public override void Update()
        {
            /// Быстрый поиск ближайших энтити 
            foreach (var item in entities.GetNearby(GalaxyVector3.Zero,10))
            {
            //    Log.Info("GetNearby", "entity id:" + item.netID);
                 
            }
            /*
            tick++;
            if(tick%50 == 0)
            {
                ExampleEntityTest test = new ExampleEntityTest(this);
                test.transform.position = new GalaxyVector3(random.Next(-10, 10), 0, random.Next(-10, 10));
                test.Init();
            }
            */
        }
    }
}
