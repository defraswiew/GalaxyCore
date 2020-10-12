using GalaxyCoreCommon;
using GalaxyCoreServer;
 
namespace SimpleMmoServer.RPGTemplate
{
    public class MobTurtle : Mob
    {
        public MobTurtle(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "MobTurtle";
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
                        heal -= damage;
                        if (heal <= 0) Death();
                    }
                    break;
            }
        }

        public override bool OnAttack()
        {
            return true;
        }

        public override void OnDestroy()
        {
            Drop drop = new Drop(instance, transform.position);
            drop.Init();
            RemoveInSpawner();
        }

        public override void Start()
        {
            InvokeRepeating("RandomPoint", 5, 60);
            galaxyVars.RegistrationClass(this);
        }

        public override void Update()
        {
            Attack();
            RandomMove();
        }
    }
}
