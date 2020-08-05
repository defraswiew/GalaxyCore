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
        public string uitext;

        [GalaxyVar(50)]
        public int ammo;

        [GalaxyVar(80)]
        public float speed;

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
            //  example = new ExampleFollow(instance);
            //   example.target = this;
            //    example.Init();
            lossOwner = NetEntityLossOwnerLogic.setServer;
            galaxyVars.RegistrationClass(this);
            InvokeRepeating("RandomInt", 1, 1);
            galaxyVars.OnChangedValue += GalaxyVars_OnChangedValue;
        }

        private void GalaxyVars_OnChangedValue(byte id)
        {
            Log.Info("Change :", id.ToString());
            Console.WriteLine();
            foreach (var item in galaxyVars.GetData(id))               
            {
                Console.Write(item.ToString());
            }
            Console.WriteLine();
        }

        public void RandomInt()
        {
            //  testValue = rand.Next(0, 999999);
        //    Log.Debug("test2", testValue.ToString());
        }

        public override void Update()
        {
          
        }

        ExampleFollow example;
        Random rand = new Random();
        public ExampleVideo(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            lossOwner = NetEntityLossOwnerLogic.setRandomClient;
        }
    }
}
