using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleEmptyInstance : Instance
    {
        public override void InMessage(byte code, byte[] data, BaseClient clientConnection)
        {
            if (Owner == clientConnection.Id) ChangeOwner();
        }


        public override void Start()
        {
            Log.Info("ExampleEmptyInstance", "instance id:" + Id);
            SetFrameRate(30);
        }

        public override void Update()
        {
        }
    }
}