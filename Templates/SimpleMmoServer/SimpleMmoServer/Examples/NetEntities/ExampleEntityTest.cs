using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleEntityTest : NetEntity
    {
        public ExampleEntityTest(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
         
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
           
        }

        protected override void OnDestroy()
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

        protected override void Update()
        {
        
        }
    }
}
