using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

public class UnityNetEntity : MonoBehaviour
{
    /// <summary>
    /// Ссылка на сетевую сущность в ядре
    /// </summary>
    public ClientNetEntity netEntity = new ClientNetEntity();
    /// <summary>
    /// буфер дополнительный данных который можно приложить при инициализации
    /// </summary>
    [HideInInspector]
    public byte[] data = null;
    /// <summary>
    /// Время инициализации объекта
    /// </summary>
    [HideInInspector]
    public float initTime;
    /// <summary>
    /// массив компонентов
    /// </summary>
    private Component[] components;

    void Awake()
    {
        // подписываемся на событие сетевого старта
        netEntity.OnNetStart += OnNetStart;
        // подписываемся на событие сетевого уничтожения
        netEntity.OnNetDestroy += OnNetDestroy;
 
    }

    void Start()
    {
        // собираем компоненты
        components = GetComponents<Component>();
        foreach (var item in components)
        {
            // регистрируем компоненты в системе управления сетевыми переменными
            netEntity.galaxyVars.RegistrationClass(item);
        }
        // Init необходимо вызывать именно в Start т.к внутренняя инициализация не успевает сработать к Awake или OnEnable
        Init();       
    }

    void Init()
    {       
        if (!netEntity.isInit)
        {
            // если netEntity не инициализирован к вызову инит значит это наш объект
            // и нужно отправлять запрос на создание сетевого экземпляра            
            // убираем все лишнее из имени
            netEntity.prefabName = gameObject.name.Split(new char[] { ' ', '(' })[0];
            // записываем текущую позицию и поворот
            netEntity.transform.position = transform.position.NetworkVector3();
            netEntity.transform.rotation = transform.rotation.NetworkQuaternion();
            // отправляем запрос на создание сетевого объекта
            GalaxyApi.netEntity.Instantiate(netEntity);                  
        }              
    }

    private void OnNetStart()
    {
        // записываем время инициализации
        initTime = Time.time;
    }

    private void OnNetDestroy()
    {
        //Удаляем объект согласно команде сервера
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // отписываемся от слушаемых событий
        netEntity.OnNetStart -= OnNetStart;
        netEntity.OnNetDestroy -= OnNetDestroy;
        // если же мы удаляем объект по своей инициативе то сообщяем об этом сетевой сущности
        if (netEntity!=null) netEntity.Destroy();
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (GalaxyNetworkController.api == null) return;
        if(GalaxyNetworkController.api.drawLables) UnityEditor.Handles.Label(transform.position + Vector3.up, "NetEntity: " + netEntity.netID);
    }
#endif

}
