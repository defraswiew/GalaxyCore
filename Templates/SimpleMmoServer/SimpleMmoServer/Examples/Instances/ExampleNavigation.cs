using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleNavigation : Instance
    {
        internal GalaxyMap map { get; private set; }
        private string path = "NavigationExample.gnm";
        public override void Close()
        {
             
        }

        public override void IncomingClient(Client clientConnection)
        {
            
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
           
        }

        public override void OutcomingClient(Client clientConnection)
        {
           
        }

        public override void Start()
        {
            Log.Info("Instance","Start ExampleNavigation");
            map = BaseMessage.Deserialize<GalaxyMap>(File.ReadAllBytes(path));
            map?.Init();
        }

        public override void Update()
        {
            map?.Update();
        }
    }
}
