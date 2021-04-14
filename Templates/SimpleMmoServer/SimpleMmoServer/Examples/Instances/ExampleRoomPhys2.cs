using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.NetEntities;
using System;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleRoomPhys2 : InstanceOpenWorldOctree
    {
        private int _max = 1;

        public override void Start()
        {
            Log.Info("ExampleRoomPhys2", "instance id:" + Id);
            SetFrameRate(20);
            Physics.Activate("phys/ExamplePhys2.phys");
        }

        public void CreateCube()
        {
            var box = new Examples.NetEntities.ExampleSphere(this, new GalaxyVector3(4, 10, 5),
                new GalaxyQuaternion(4, 10, 20, 0.5f));
            box.transform.Position = new GalaxyVector3(4, 10, 5);
            box.Init();
            _max--;
            if (_max < 1) CancelInvoke("CreateCube");
        }

        public override void IncomingClient(Client clientConnection)
        {
            Invoke("TestEntity", 2, clientConnection);
        }

        public void TestSend(Client clientConnection)
        {
            clientConnection.SendMessage(105, Array.Empty<byte>(), GalaxyDeliveryType.reliable);
        }

        public void TestEntity(Client clientConnection)
        {
            ExampleVideo entity = new ExampleVideo(this);
            entity.ChangeOwner(clientConnection);
            entity.PrefabName = "ExampleVideo";
            entity.Init();
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
        }

        public override void Update()
        {
        }
    }
}