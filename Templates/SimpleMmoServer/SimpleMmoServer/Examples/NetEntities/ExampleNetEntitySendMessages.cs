using System;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleNetEntitySendMessages: NetEntity
    {
        public ExampleNetEntitySendMessages(Instance instance, GalaxyVector3 position = new GalaxyVector3(), GalaxyQuaternion rotation = new GalaxyQuaternion()) : base(instance, position, rotation)
        {
        }

        protected override void OnRemotePosition(GalaxyVector3 remotePosition)
        {

        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {

        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {

        }

        protected override void Start()
        {
             InvokeRepeating(nameof(TestSendToOwner),1,1);
        }

        protected override void OnDestroy(byte[] customData)
        {

        }

        protected override void Update()
        {
            var message = new GalaxyBitWriter(new byte[128]);
            message.WriteSingle(Instance.Time.Time);
            SendMessage(2,message.WrittenSpan.ToArray(),GalaxyDeliveryType.reliable);
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient baseClientSender)
        {
           SendMessage(externalCode,data,GalaxyDeliveryType.reliableOrdered,false,10);
        }

        public void TestSendToOwner()
        {
            var message = new GalaxyBitWriter(new byte[128]);
            message.WriteString(DateTime.UtcNow.ToString("O"));
            SendToOwner(1,message.WrittenSpan.ToArray(),GalaxyDeliveryType.reliable);
        }
    }
}
