using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.NetEntitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleRoomPhys2 : InstanceOpenWorld
    {
        int max = 1;
        public override void Start()
        {
            Log.Info("ExampleRoomPhys2", "instance id:" + id);
            SetFrameRate(40);
            physics.Activate("phys/ExamplePhys2.phys");
            InvokeRepeating("CreateCube", 1, 1);
        }

        public void CreateCube()
        {
            Examples.NetEntitys.ExampleSphere box = new Examples.NetEntitys.ExampleSphere(this, new GalaxyVector3(4, 10, 5), new GalaxyQuaternion(4, 10, 20, 0.5f));
            box.transform.position = new GalaxyVector3(4, 10, 5);
            box.Init();
            max--;
            if (max < 1) CancelInvoke("CreateCube");
        }


        public override void Close()
        {
          
        }

        public override void IncomingClient(Client clientConnection)
        {          
            Invoke("TestEntity", 2, clientConnection);
        }
        public void TestEntity(Client clientConnection)
        {
            ExampleVideo entity = new ExampleVideo(this);
            entity.prefabName = "ExampleVideo";
          //  entity.ChangeOwner(clientConnection);
            //entity.lossOwner = NetEntityLossOwnerLogic.destroy;
            entity.Init();
        }

     


        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
          
        }

        public override void OutcomingClient(Client clientConnection)
        {
            
        }

        public override void Update()
        {

        }

   
    }
}
