using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate;


namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример реализации моба босса, с масс супер атакой
    /// </summary>
    public class MobSlimeBoss : Mob
    {
        int skill1Count = 0;
        public MobSlimeBoss(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "MobSlimeBoss";
            syncType = NetEntityAutoSync.position;
            heal = 1000;
            maxHeal = 1000;
            attackDistanse = 5;
            minDamage = 3;
            maxDamage = 10;
            moveSpeed = 0.3f;
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
            // каждая 8 атака будет супер атакой по площади
            if (attackCount % 8 != 0) return true;
            state = (byte)MobState.attack;
            int damage = GRand.NextInt(minDamage * 2, maxDamage * 3);
            BitGalaxy message = new BitGalaxy();
            message.WriteValue(damage);
            // отправляем сообщение о том что мы наносим кому то дамаг. 
            SendMessage(200, message.data, GalaxyDeliveryType.reliable);
            InvokeRepeating("SkillDamage", 0.5f, 1, damage);
            return false;
        }

        public void SkillDamage(int damage)
        {
            skill1Count++;
            if (skill1Count >= 4)
            {
                CancelInvoke("SkillDamage");
                skill1Count = 0;
            }

            RPGTemplatePlayer player;
            // ищем все объекты в радиусе 6 метров
            foreach (var item in instance.entities.GetNearby(transform.position, 10))
            {
                player = item as RPGTemplatePlayer;
                // если это игрок то сообщяем ему дамаг
                if (player != null)
                {
                    player.SetDamage(damage);
                }
            }

        }

        public override void OnDestroy()
        {
            RemoveInSpawner();
            Drop drop = new Drop(instance, transform.position);
            drop.Init();
        }

        public override void Start()
        {
            InvokeRepeating("RandomPoint", 5, 120);
            galaxyVars.RegistrationClass(this);
        }

        public override void Update()
        {
            Attack();
            RandomMove();
        }
    }
}