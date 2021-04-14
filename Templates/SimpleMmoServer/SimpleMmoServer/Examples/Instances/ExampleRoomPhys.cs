using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Комната демонстрирующая работу с физикой.
    /// </summary>
    public class ExampleRoomPhys : Instance
    {
        private float _timer = -3;
        private int _bodyForceCount;
        private int _bodyForceMax = 1000;
        public GalaxyVector3 ForceTarget = new GalaxyVector3(10, 10, 10);
        private int _frameCount;

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
        }

        public override void Start()
        {
            Log.Info("ExampleRoomPhys", "instance id:" + Id); // выводим в консоль тип комнаты
            SetFrameRate(20); // устанавливаем подходящий врейм рейт
            //   physics.Activate("phys/ExamplePhys.phys"); // активизуем физику c указанием пути на файл запеченой сцены    
            Physics.Activate();
        }

        public override void Update()
        {
            _timer += Time.DeltaTime;

            if (_timer > 0.2f)
            {
                _timer = 0;
                //    if (bodyCount < bodyMax) BoxSpawn();
                if (_bodyForceCount < _bodyForceMax) BodyForceSpawn();
            }

            _frameCount++;
            if (_frameCount % 300 == 0)
            {
                ForceTarget = new GalaxyVector3(GRand.NextInt(0, 100), GRand.NextInt(0, 100), GRand.NextInt(0, 100));
            }
        }

        private void BodyForceSpawn()
        {
            var entity = new Examples.NetEntities.ExampleForce(this,
                new GalaxyVector3(GRand.NextInt(5, 10), GRand.NextInt(5, 10), GRand.NextInt(5, 10)),
                new GalaxyQuaternion(0, 0, 0, 0)) {Room = this};
            entity.Init();
            _bodyForceCount++;
        }
    }
}