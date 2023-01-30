using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate.Mobs;

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример реализации моба слизня
    /// </summary>
    public class MobSlime : Mob
    {
        public MobSlime(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            // указываем имя префаба
            PrefabName = "MobSlime";
            // указываем тип синхронизации
            // нас устраивает только позиция
            // поворот будем высчитывать по направлению движения
           
            ChangeOwner();
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
            switch (externalCode)
            {
                // сообщение о нанесении урона
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
            RemoveInSpawner();
            Drop drop = new Drop(Instance, transform.Position);
            drop.Init();
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
            GalaxyVars.AutoApplyRemoteData = false;
        }

        protected override void Update()
        {
            Attack();
            RandomMove();
        }
    }
}