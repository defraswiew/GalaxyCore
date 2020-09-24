using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleVideo : NetEntity
    {
        [GalaxyVar(1)]
        public string text;
        [GalaxyVar(2)]
        public int hp;

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
            SendMessageExcept(clientSender, externalCode, data);
        }

        public override void OnDestroy()
        {
        //    example.Destory();
        }

        public override void Start()
        {
            
           
        //    Invoke("Test", 1f);
       
        }
        public void Test()
        {
            galaxyVars.ForceSync();
        }
      
       

        public override void Update()
        {
      
        }

       
        public ExampleVideo(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            text = "sfgsaassgfg";
            hp = 1234;
            galaxyVars.RegistrationClass(this);
        }
    }
}
