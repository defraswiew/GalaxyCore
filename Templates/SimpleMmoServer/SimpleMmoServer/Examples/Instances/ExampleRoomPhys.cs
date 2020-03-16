using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;
using SimpleMmoServer.Examples.NetEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Комната демонстрирующая работу с физикой.
    /// </summary>
    public class ExampleRoomPhys : Instance
    {
        float timer = -3; // 
        int bodyCount; // текущее число боксов
        int bodyMax = 500; // целевое число боксов       
        int bodyForseCount;
        int bodyForseMax = 1;
        public GalaxyVector3 forseTarget = new GalaxyVector3(10, 10, 10);
        int frameCount;
        //////// RayCast TEST //////
        private ExamplePlayer player;
        private Examples.NetEntitys.ExamplePlayer debug;
        private GalaxyVector3 directional = new GalaxyVector3();
        private GalaxyVector3 startPoint = new GalaxyVector3(-9,5,6);        
        //////////////////////////////
        public override void OutcomingClient(Client clientConnection)
        {
          
        }

        public override void Close()
        {
           
        }

        public override void IncomingClient(Client clientConnection)
        {
         
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
          
        }

        public override void Start()
        {
            Log.Info("ExampleRoomPhys", "instance id:"+id);// выводим в консоль тип комнаты
            SetFrameRate(20); // устанавливаем подходящий врейм рейт
            // physics.Activate("phys/ExamplePhys.phys"); // активизуем физику c указанием пути на файл запеченой сцены    
             physics.Activate();
        }

        public override void Update()
        {
            timer += Time.deltaTime;

            if (timer > 0.2f)
            {
                timer = 0;
                if (bodyCount < bodyMax) BoxSpawn();
                if (bodyForseCount < bodyForseMax) BodyForseSpawn();
            }

            frameCount++;
            if (frameCount % 300 == 0)
            {
                forseTarget = new GalaxyVector3(GRand.NextInt(0, 100), GRand.NextInt(0, 100), GRand.NextInt(0, 100));
            }

            if (player == null)
            {
                player = (ExamplePlayer)entities.list.Where(x => x.prefabName == "Player").FirstOrDefault();
                if (player != null)
                {
                    debug = new NetEntitys.ExamplePlayer(this);
                    debug.Init();
                    debug.physics.Deactivate();
                }
                return;
            }
            RayTest();
        }

        /// <summary>
        /// пример использования рейкаста (проверяем достает ли луч до игрока)
        /// </summary>
        private void RayTest()
        {
            directional =  player.transform.position - startPoint;           
            physics.RayCast(startPoint,directional, out RaycastHit raycastHit);
            debug.transform.position = raycastHit.point;
            if (raycastHit.entity == player)
            {
                player.SendRayCastResult(true);
                Log.Info("Distanse", raycastHit.Distanse().ToString());
            }
            else
            {
                player.SendRayCastResult(false);
                Log.Info("Tag: ", raycastHit.tag);
            }         
        }


        private void BoxSpawn()
        {          

                if(bodyCount % 3 == 0) {

                    Examples.NetEntitys.ExampleSphere sphere = new Examples.NetEntitys.ExampleSphere(this, new GalaxyVector3(0.36f, 15, 17.5f), new GalaxyQuaternion(4, 10, 20, 0.5f));
                    sphere.Init();
                    bodyCount++;
                    return;
                }
            if (bodyCount % 4 == 0)
            {

                Examples.NetEntitys.ExamplePhysCapsule capsule = new Examples.NetEntitys.ExamplePhysCapsule(this, new GalaxyVector3(0.36f, 15, 17.5f), new GalaxyQuaternion(4, 10, 20, 0.5f));
                capsule.Init();
                bodyCount++;
                return;
            }
            Examples.NetEntitys.ExamplePhysBox box = new Examples.NetEntitys.ExamplePhysBox(this, new GalaxyVector3(0.36f, 15, 17.5f),new GalaxyQuaternion(4, 10, 20, 0.5f));
                    box.Init();
                   bodyCount++;
        }

        private void BodyForseSpawn()
        {
            Examples.NetEntitys.ExampleForse entity = new Examples.NetEntitys.ExampleForse(this, new GalaxyVector3(GRand.NextInt(5, 10), GRand.NextInt(5, 10), GRand.NextInt(5, 10)), new GalaxyQuaternion(0, 0, 0, 0));
            entity.room = this;
            entity.Init();
            bodyForseCount++;
        }
    }
}
