using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Пример создания множества авторитарно двигаемых объектов
    /// </summary>
    public class ExampleRoomMovers : Instance
    {
        private int _moverCount;
        private int _moverMax = 1000;

        public override void Start()
        {
            SetFrameRate(5);
            InvokeRepeating("Spawn", 1, 0.2f);
        }
        public override void InMessage(byte code, byte[] data, Client client)
        {
            Log.Debug("Instance", "InMessage code:" + code);
        }

        public void Spawn()
        {
            if (_moverCount > _moverMax) CancelInvoke("Spawn");
            Examples.NetEntities.ExampleRandomMove mover = new Examples.NetEntities.ExampleRandomMove(this, new GalaxyVector3(0, 1, 0));
            mover.Init();
            _moverCount++;
        }

        public override void Update()
        {
        }
    }
}


