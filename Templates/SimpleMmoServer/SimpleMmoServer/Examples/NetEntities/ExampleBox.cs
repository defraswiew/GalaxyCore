using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleBox : NetEntity
    {
        public ExampleBox(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
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
             
        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {
            
        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {
             
        }

        protected override void Start()
        {
            InvokeRepeating("ChangeStatic", 5, 5);
            InvokeRepeating("POS", 1, 1);
        }

        public void ChangeStatic()
        {
            if (IsStatic)
            {
                IsStatic = false;
            }
            else
            {
                IsStatic = true;
            }

            Log.Info("ChangeStatic", IsStatic.ToString());
        }

        public void POS()
        {
            Log.Debug("Position", transform.Position.ToString());
        }

        protected override void Update()
        {
            Log.Debug("Update", "Update");
        }
    }
}