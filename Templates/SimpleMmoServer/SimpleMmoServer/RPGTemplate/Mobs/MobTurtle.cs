using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate.Mobs;

namespace SimpleMmoServer.RPGTemplate
{
    public class MobTurtle : Mob
    {
        public MobTurtle(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "MobTurtle";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
            switch (externalCode)
            {
                case 200:
                    {
                        var message = new GalaxyBitReader(data);
                        if (!message.TryReadInt32(out int damage)) return;
                        Heal -= damage;
                        if (Heal <= 0) Death();
                    }
                    break;
            }
        }

        public override bool OnAttack()
        {
            return true;
        }

        protected override void OnDestroy(byte[] customData)
        {
            Drop drop = new Drop(Instance, transform.Position);
            drop.Init();
            RemoveInSpawner();
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
            InvokeRepeating("RandomPoint", 5, 60);
            GalaxyVars.RegistrationClass(this);
        }

        protected override void Update()
        {
            Attack();
            RandomMove();
        }
    }
}
