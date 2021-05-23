using System.IO;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.NetEntities;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleNavigation:Instance
    {
        public GalaxyMap Map;
        
        private int _moverCount;
        private int _moverMax = 80;

        public override void Start()
        {
            Map = BaseMessage.Deserialize<GalaxyMap>(File.ReadAllBytes("D:/ExampleNav.gnm"));
            Map?.Init();
            Map.SearchNodeDistance = 15;
            SetFrameRate(20);
            InvokeRepeating("Spawn", 1, 3f);
        }
 
        
        public override void InMessage(byte code, byte[] data, BaseClient client)
        {
            Log.Debug("Instance", "InMessage code:" + code);
        }

        public void Spawn()
        {
            if (_moverCount > _moverMax) CancelInvoke("Spawn");
            var mover = new NavigationMob(this, new GalaxyVector3(1, 1, 1));
            mover.Init();
            _moverCount++;
        }
        
        public override void Update()
        {
           Map?.Update();
        }

    }
}