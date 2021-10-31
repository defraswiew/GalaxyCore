using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.NetEntities;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleRoomMovers : Instance
    {
        private int _moverCount;
        private int _moverMax = 1000;

        public override void Start()
        {
            SetFrameRate(2);
            InvokeRepeating("Spawn", 1, 0.2f);
        }
        public override void InMessage(byte code, byte[] data, BaseClient client)
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

        public override void IncomingClient(BaseClient clientConnection)
        {
            var entity = new ExampleNetVars(this);
            entity.ChangeOwner(clientConnection);
            entity.Init();
        }
    }
}


