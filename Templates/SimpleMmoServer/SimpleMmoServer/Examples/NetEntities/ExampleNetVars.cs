using System;
using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleNetVars:NetEntity
    {
        [GalaxyVar(1,false)]
        public string Name;
        [GalaxyVar(2)]
        public int Hp;
        [GalaxyVar(3)]
        public float ServerTime;

        private Random _rnd;

        public ExampleNetVars(Instance instance, GalaxyVector3 position = new GalaxyVector3(), GalaxyQuaternion rotation = new GalaxyQuaternion()) : base(instance, position, rotation)
        {
            PrefabName = "ExampleVideo";
            _rnd = new Random();
            GalaxyVars.RegistrationClass(this);
            Name = _rnd.Next(0, 100000).ToString();
            Hp = 100;
        }

        
        protected override void Update()
        {
            ServerTime = Instance.Time.Time;
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
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

        protected override void Start()
        {
          
        }

        protected override void OnDestroy()
        {
           
        }

    }
}