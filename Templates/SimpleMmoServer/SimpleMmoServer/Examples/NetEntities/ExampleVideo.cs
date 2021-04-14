using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleVideo : NetEntity
    {
        [GalaxyVar(1)] public string Text;
        [GalaxyVar(2)] public int Hp;

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
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
            newNetEntity.ChangeOwner(OwnerClient);
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
            Log.Info("Position", transform.Position.ToString());
        }

        public ExampleVideo(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default,
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