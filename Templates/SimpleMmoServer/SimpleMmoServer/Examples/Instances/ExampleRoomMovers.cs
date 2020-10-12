using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Пример создания множества авторитарно двигаемых объектов
    /// </summary>
    public class ExampleRoomMovers : Instance
    {
        int moverCount;
        int moverMax = 50000;
        public override void OutcomingClient(Client client)
        {

        }

        public override void Close()
        {

        }

        public override void IncomingClient(Client client)
        {

        }

        public override void Start()
        {
            SetFrameRate(10);
            InvokeRepeating("Spawn", 1, 0.2f);
        }
        public override void InMessage(byte code, byte[] data, Client client)
        {
            Log.Debug("Instance", "InMessage code:" + code);
        }

        public void Spawn()
        {
            if (moverCount > moverMax) CancelInvoke("Spawn");
            Examples.NetEntitys.ExampleRandomMove mover = new Examples.NetEntitys.ExampleRandomMove(this, new GalaxyVector3(0, 1, 0));
            mover.Init();
            moverCount++;
        }

        public override void Update()
        {
        }

    }
}


