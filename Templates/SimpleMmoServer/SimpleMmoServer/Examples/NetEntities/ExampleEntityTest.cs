using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleEntityTest : NetEntity
    {
        public ExampleEntityTest(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
         
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
           
        }

        protected override void OnDestroy()
        {
             
        }

        protected override void Start()
        {
       
        }

        protected override void Update()
        {
        
        }
    }
}
