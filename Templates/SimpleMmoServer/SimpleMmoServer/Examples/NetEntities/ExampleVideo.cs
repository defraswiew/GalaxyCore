using GalaxyCoreCommon;
using GalaxyCoreServer;
#if GALAXY_DOUBLE
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleVideo : NetEntity
    {
        [GalaxyVar(1)] public string Text;
        [GalaxyVar(2)] public int Hp;
        [GalaxyVar(3)] public float Time;

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
            SendMessageExcept(clientSender, externalCode, data);
        }

        protected override void Start()
        {
        }

        public void Kill()
        {
            Destory();
        }

        protected override void OnDestroy()
        {
            var newNetEntity = new ExampleVideo(Instance, transform.Position);
            newNetEntity.ChangeOwner();
            newNetEntity.Init();
        }

        public void Send()
        {
        }

        public void SendOcto()
        {
            Log.Info("SendOcto", "sens");
            SendMessageByOctoVisible(101, new byte[0], GalaxyDeliveryType.reliable);
        }

        public void Test()
        {
            GalaxyVars.ForceSync();
        }

        protected override void Update()
        {
            Time = Instance.Time.Time;
            Log.Info("Test", Hp.ToString());
        }

        public ExampleVideo(Instance instance, vector position = default, GalaxyQuaternion rotation = default,
            NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation,
            syncType)
        {
            PrefabName = "ExampleVideo";
            Text = "hello";
            Hp = 666;
            GalaxyVars.RegistrationClass(this);
        }
    }
}