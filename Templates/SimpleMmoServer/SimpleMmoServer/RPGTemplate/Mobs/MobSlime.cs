using GalaxyCoreCommon;
using GalaxyCoreServer;
 
namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример реализации моба слизня
    /// </summary>
    public class MobSlime : Mob
    {
        public MobSlime(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            // указываем имя префаба
            prefabName = "MobSlime";
            // указываем тип синхронизации
            // нас устраивает только позиция
            // поворот будем высчитывать по направлению движения
            syncType = NetEntityAutoSync.position;
             
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
            switch (externalCode)
            {
                // сообщение о нанесении урона
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
            RemoveInSpawner();
            Drop drop = new Drop(instance, transform.position);
            drop.Init();
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
