using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleChangeOwner : NetEntity
    {
        private int _timer = 0;
        public BaseClient OldOwner;

        public ExampleChangeOwner(Instance instance, GalaxyVector3 position = default,
            GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) :
            base(instance, position, rotation, syncType)
        {
            FullUpdateRate = 5;
            
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
            OldOwner = Instance.GetClientById(OwnerClientId);
            InvokeRepeating("Test", 10, 2, 5);
        }

        public void Test(int num)
        {
            Log.Info("Invoke", "test");
        }

        protected override void Update()
        {
        }
    }
}