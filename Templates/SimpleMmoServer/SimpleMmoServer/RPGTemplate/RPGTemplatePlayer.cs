using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoServer.RPGTemplate.Mobs;

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример игрока
    /// </summary>
    public class RPGTemplatePlayer : NetEntity
    {
        /// <summary>
        /// Скорость движения персонажа
        /// </summary>
        [GalaxyVar(1)] public float Move;

        /// <summary>
        /// Текущие жизни
        /// </summary>
        [GalaxyVar(10)] public int Heal = 100;

        /// <summary>
        /// Максимальный хп
        /// </summary>
        [GalaxyVar(11)] public int MaxHeal = 100;

        public RPGTemplatePlayer(Instance instance, GalaxyVector3 position = default,
            GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) :
            base(instance, position, rotation, syncType)
        {
            PrefabName = "RPGTemplatePlayer";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
            // переоправляем сообщение всем кто видит эту сущность
            SendMessageByOctoVisible(externalCode, data, GalaxyDeliveryType.reliable);
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
            // дергаем метод поиска мобов раз в секунду
            // использовать InvokeRepeating значительно дешевле чем считать время в Update
            InvokeRepeating("MobFinder", 1, 1);
            InvokeRepeating("StatControl", 1, 1);
            GalaxyVars.RegistrationClass(this);
        }

        protected override void Update()
        {
        }

        public void StatControl()
        {
            if (Heal < MaxHeal) Heal++;
        }

        public void MobFinder()
        {
            // смотрим нет ли мобов в радиусе 20 метров
            // искать мобов игроками дешевле чем мобами игроков
            Mob mob;
            foreach (var item in Instance.Entities.GetNearby(transform.Position, 20))
            {
                mob = item as Mob;
                if (mob != null)
                {
                    mob.PlayerNear(this);
                }
            }
        }

        /// <summary>
        /// Нанесение урона
        /// </summary>
        /// <param name="damage"></param>
        public void SetDamage(int damage)
        {
            Heal -= damage;
        }
    }
}