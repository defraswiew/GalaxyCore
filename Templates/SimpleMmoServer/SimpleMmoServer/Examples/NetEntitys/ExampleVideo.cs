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
        public override void Start()
        {
            //   InvokeRepeating("Send", 2f, 2f);
            //  InvokeRepeating("SendOcto", 1f, 2f);
            Invoke("Kill", 10);
        }
        public void Kill()
        {
            Destory();
        }
        public override void OnDestroy()
        {
            var newNetEntity = new ExampleVideo(instance, transform.position);
            newNetEntity.ChangeOwner(ownerClient);
            newNetEntity.Init();
        }



        public void Send()
        {
            SendMessage(100, new byte[0], GalaxyDeliveryType.reliable);
        }
        public void SendOcto()
        {
            Log.Info("SendOcto", "sens");
            SendMessageByOctoVisible(101, new byte[0], GalaxyDeliveryType.reliable);
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
            prefabName = "ExampleVideo";
            text = "hello";
            hp = 666;
            galaxyVars.RegistrationClass(this);
        }
    }
}
