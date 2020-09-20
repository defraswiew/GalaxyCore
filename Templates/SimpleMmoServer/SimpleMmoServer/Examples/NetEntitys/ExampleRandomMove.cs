using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoCommon;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleRandomMove : NetEntity
    {     
        private GalaxyVector3 target = new GalaxyVector3(); // точка к которой будем двигаться       
        private float timer; // текущий таймер шага
        private int randTime; // время ожидания шага
               

        public ExampleRandomMove(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "Move"; // задаем имя, что бы клиент знал какой префаб отображать     
        }

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
          
        }

        public override void OnDestroy()
        {
           
        }

        public override void Start()
        {
           transform.syncType = NetEntityAutoSync.position_and_rotation; // указывем тип автоматической синхронизации   
            
        }

        public override void Update()
        {
            timer += instance.Time.deltaTime;                       
            if (timer > randTime)
            {
                timer = 0;
                randTime = GRand.NextInt(20, 45);                 
                target.x = GRand.NextInt(-180, 180);
                target.z = GRand.NextInt(-180, 180);
            }
            transform.position = GalaxyVector3.Lerp(transform.position, target, instance.Time.deltaTime*0.03f);   // лерпим текущую позицию к целевой раз в кадр         
        }

         
    }
}
