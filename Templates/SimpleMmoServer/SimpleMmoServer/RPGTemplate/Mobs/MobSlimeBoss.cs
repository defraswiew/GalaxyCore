using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate;
using SimpleMmoServer.RPGTemplate.Mobs;

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример реализации моба босса, с масс супер атакой
    /// </summary>
    public class MobSlimeBoss : Mob
    {
        private int _skill1Count = 0;
        
        public MobSlimeBoss(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "MobSlimeBoss";
            Heal = 1000;
            MaxHeal = 1000;
            attackDistanse = 5;
            minDamage = 3;
            maxDamage = 10;
            MoveSpeed = 0.3f;
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
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
            // каждая 8 атака будет супер атакой по площади
            if (attackCount % 8 != 0) return true;
            State = (byte)MobState.attack;
            int damage = GRand.NextInt(minDamage * 2, maxDamage * 3);
            BitGalaxy message = new BitGalaxy();
            message.WriteValue(damage);
            // отправляем сообщение о том что мы наносим кому то дамаг. 
            SendMessage(200, message.Data, GalaxyDeliveryType.reliable);
            InvokeRepeating("SkillDamage", 0.5f, 1, damage);
            return false;
        }

        public void SkillDamage(int damage)
        {
            _skill1Count++;
            if (_skill1Count >= 4)
            {
                CancelInvoke("SkillDamage");
                _skill1Count = 0;
            }

            RPGTemplatePlayer player;
            // ищем все объекты в радиусе 6 метров
            foreach (var item in Instance.Entities.GetNearby(transform.Position, 10))
            {
                player = item as RPGTemplatePlayer;
                // если это игрок то сообщяем ему дамаг
                if (player != null)
                {
                    player.SetDamage(damage);
                }
            }

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
            InvokeRepeating("RandomPoint", 5, 120);
            GalaxyVars.RegistrationClass(this);
        }

        protected override void Update()
        {
            Attack();
            RandomMove();
        }
    }
}