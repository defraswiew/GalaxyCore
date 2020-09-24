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
        int sizeX = 20;
        int sizeZ = 20;
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
            if (clients.Count == 0) mapSaver.SaveInstance(this, name);
        }

        public override void Start()
        {
           
            autoClose = false;
            visibleDistance = 40;
            name = "Building example";
            mapSaver.Load(this,name);
            /*
          

            for (int x = 0; x < sizeX; x+=2)
            {
                for (int z = 0; z < sizeZ; z += 2)
                {
                    NetEntityStandart netEntity = new NetEntityStandart(this);
                    netEntity.prefabName = "Bld_wall_1";
                    netEntity.isStatic = true;
                    netEntity.lossOwner = NetEntityLossOwnerLogic.setServer;
                    netEntity.transform.position.x = x;
                    netEntity.transform.position.z = z;
                    netEntity.Init();
                }
            }
            */
      
        }
      
        public override void Update()
        {
           
        }
    }
}
