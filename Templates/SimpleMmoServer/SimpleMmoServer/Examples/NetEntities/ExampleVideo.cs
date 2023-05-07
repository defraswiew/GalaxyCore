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
        [GalaxyVar(4)]
        public float TestUser;
        [GalaxyVar(5,false,false)]
        public float TestUserNotTake;

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
        
        protected override void OnRemotePosition(GalaxyVector3 remotePosition)
        {
            transform.Position = remotePosition;
        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {
            transform.Scale = remoteScale;
        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {
            transform.Rotation = remoteRotation;
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
        //    Log.Info("Test", Hp.ToString());
        }

        public ExampleVideo(Instance instance, vector position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "ExampleVideo";
            Text = "hello";
            Hp = 666;
            GalaxyVars.RegistrationClass(this);
            GalaxyVars.OnChangedValue+=OnChangedValue;
        }

        private void OnChangedValue(byte id)
        {
           Log.Info("OnChangedValue",id.ToString());
        }
    }
}