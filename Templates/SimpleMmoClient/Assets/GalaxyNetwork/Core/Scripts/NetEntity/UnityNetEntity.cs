using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private Component[] components;

    void Awake()
    {
        netEntity.OnNetStart += OnNetStart;
        netEntity.OnNetDestroy += OnNetDestroy;
    }
    void Start()
    {
        components = GetComponents<Component>();
        // Init необходимо вызывать именно в Start т.к внутренняя инициализация не успевает сработать к Awake или OnEnable
        Init();       
    }

    void Init()
    {
     //   netEntity.OnNetStart += OnNetStart;
     //   netEntity.OnNetDestroy += OnNetDestroy; 
       
        if (!netEntity.isInit)
        {
            // если netEntity все еще null то это явно наш объект
            // и нужно отправлять запрос на создание сетевого экземпляра
            // создаем пустой экземпляр сетевой сущности
          //  netEntity = new ClientNetEntity();            
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
     //   Debug.Log(gameObject.name + " OnNetStart");
        //задаем время инициализации
        initTime = Time.time;
    }

    private void OnNetDestroy()
    {
        //Удаляем объект согласно команде сервера
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
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



    void LoadRPC()
    {
        List<MemberInfo> options_rpc = new List<MemberInfo>();
        options_rpc = components.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(n => Attribute.IsDefined(n, typeof(NetEntityRPC))).ToList();
    }
}
