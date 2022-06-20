using System;
using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleNetEntitySendMessages: NetEntity
    {
        public ExampleNetEntitySendMessages(Instance instance, GalaxyVector3 position = new GalaxyVector3(), GalaxyQuaternion rotation = new GalaxyQuaternion(), NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
        }

        protected override void Start()
        {
             InvokeRepeating(nameof(TestSendToOwner),1,1);
        }

        protected override void OnDestroy()
        {
            
        }

        protected override void Update()
        {
            var message = new BitGalaxy();
            message.WriteValue(Instance.Time.Time);
            SendMessage(2,message.Data,GalaxyDeliveryType.reliable);
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient baseClientSender)
        {
          
        }

        public void TestSendToOwner()
        {
            var message = new BitGalaxy();
            message.WriteValue(DateTime.Now.ToString());
            SendToOwner(1,message.Data,GalaxyDeliveryType.reliable);
        }
    }
}