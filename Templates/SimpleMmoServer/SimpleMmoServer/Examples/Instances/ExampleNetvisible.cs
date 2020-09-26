using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleNetvisible : InstanceOpenWorldOctree
    {
        public MapSaver mapSaver = new MapSaver();
 
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
            if (clients.Count == 0) mapSaver.SaveInstance(this, "cs_mansion");
        }

        public override void Start()
        {
           
            autoClose = false;
            visibleDistance = 40;
            name = "Building example";
            mapSaver.Load(this, "cs_mansion");    
        }
      
        public override void Update()
        {
           
        }
    }
}
