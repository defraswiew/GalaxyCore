using GalaxyCoreCommon;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleBox : NetEntity
    {
        public ExampleBox(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
         
        }

        public override void OnDestroy()
        {
          
        }

        public override void Start()
        {
            InvokeRepeating("ChangeStatic", 5, 5);
            InvokeRepeating("POS", 1, 1);
        }

        public void ChangeStatic()
        {
            if (isStatic)
            {                 
                isStatic = false;
            } else
            {
                isStatic = true;
            }
            Log.Info("ChangeStatic", isStatic.ToString());
        }

        public void POS()
        {
            Log.Debug("Position", transform.position.ToString());
        }
        public override void Update()
        {

        Log.Debug("Update", "Update");
        }
    }
}
