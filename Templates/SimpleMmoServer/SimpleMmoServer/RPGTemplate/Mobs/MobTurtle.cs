using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate.Mobs;

namespace SimpleMmoServer.RPGTemplate
{
    public class MobTurtle : Mob
    {
        public MobTurtle(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            PrefabName = "MobTurtle";
            syncType = NetEntityAutoSync.position_and_rotation;
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
            switch (externalCode)
            {
                case 200:
                    {
                        BitGalaxy message = new BitGalaxy(data);
                        int damage = message.ReadInt();
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

        protected override void OnDestroy()
        {
            Drop drop = new Drop(Instance, transform.Position);
            drop.Init();
            RemoveInSpawner();
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
