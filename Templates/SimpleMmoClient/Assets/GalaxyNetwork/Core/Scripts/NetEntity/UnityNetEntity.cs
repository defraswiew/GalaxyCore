using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts.NetEntity
{
    public class UnityNetEntity : MonoBehaviour
    {
        /// <summary>
        /// Link to a network entity in the kernel
        /// </summary>
        public ClientNetEntity NetEntity = new ClientNetEntity();

        /// <summary>
        /// additional data buffer that can be attached during initialization
        /// </summary>
        [HideInInspector] 
        public byte[] Data = null;

        /// <summary>
        /// Object initialization time
        /// </summary>
        [HideInInspector] 
        public float InitTime;

        /// <summary>
        /// массив компонентов
        /// </summary>
        private Component[] _components;

        private void Awake()
        {
            // подписываемся на событие сетевого старта
            NetEntity.OnNetStart += OnNetStart;
            // подписываемся на событие сетевого уничтожения
            NetEntity.OnNetDestroy += OnNetDestroy;
        }

        void Start()
        {
            _components = GetComponents<Component>();
            foreach (var item in _components)
            {
                NetEntity.GalaxyVars.RegistrationClass(item);
            }

            Init();
        }

        void Init()
        {
            if (!NetEntity.IsInit)
            {
                // если netEntity не инициализирован к вызову инит значит это наш объект
                // и нужно отправлять запрос на создание сетевого экземпляра            
                // убираем все лишнее из имени
                NetEntity.PrefabName = gameObject.name.Split(new char[] {' ', '('})[0];
                // записываем текущую позицию и поворот
                NetEntity.transform.Position = transform.position.NetworkVector3();
                NetEntity.transform.Rotation = transform.rotation.NetworkQuaternion();
                // отправляем запрос на создание сетевого объекта
                GalaxyApi.NetEntity.Instantiate(NetEntity);
            }
        }

        private void OnNetStart()
        {
            // записываем время инициализации
            InitTime = Time.time;
        }

        private void OnNetDestroy()
        {
            //Удаляем объект согласно команде сервера
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            // отписываемся от слушаемых событий
            NetEntity.OnNetStart -= OnNetStart;
            NetEntity.OnNetDestroy -= OnNetDestroy;
            // если же мы удаляем объект по своей инициативе то сообщяем об этом сетевой сущности
            if (NetEntity != null) NetEntity.Destroy();
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (GalaxyNetworkController.Api == null) return;
            if (GalaxyNetworkController.Api.DrawLabels)
                UnityEditor.Handles.Label(transform.position + Vector3.up, "NetEntity: " + NetEntity.NetID);
        }
#endif
    }
}