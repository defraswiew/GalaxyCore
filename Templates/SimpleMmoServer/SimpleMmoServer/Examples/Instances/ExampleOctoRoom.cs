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
            for (int i = 0; i < 10; i++)
            {
                ExampleEntityTest test = new ExampleEntityTest(this);
                test.transform.position = new GalaxyVector3(random.Next(-10, 10), 0, random.Next(-10, 10));
                test.Init();
            }
            Invoke("TestInvoke", 5,10,"привет");
        }

        private void TestInvoke(int num, string name)
        {
            Log.Info("TestInvoke", "Complete " + num +  "  " +name);
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
