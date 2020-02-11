using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityNetEntity : MonoBehaviour
{
    /// <summary>
    /// Ссылка на сетевую сущность в ядре
    /// </summary>
    public ClientNetEntity netEntity = new ClientNetEntity();

  
    public int netID = 0;

    

    void Start()
    {
        // Init необходимо вызывать именно в Start т.к внутренняя инициализация не успевает сработать к Awake или OnEnable
        Init();
    }

    void Init()
    {
        if (!netEntity.isInit)
        {
            // если netEntity все еще null то это явно наш объект
            // и нужно отправлять запрос на создание сетевого экземпляра
            // создаем пустой экземпляр сетевой сущности
            netEntity = new ClientNetEntity();            
            // убираем все лишнее из имени
            netEntity.name = gameObject.name.Split(new char[] { ' ', '(' })[0];
            // записываем текущую позицию и поворот
            netEntity.position = transform.position.NetworkVector3();
            netEntity.rotation = transform.rotation.NetworkQuaternion();
            // отправляем запрос на создание сетевого объекта
            GalaxyApi.netEntity.Instantiate(netEntity);
            // подписываемся на событие об успешной инициализации
            netEntity.OnNetStart += OnNetStart; 
        }
        else
        {
            OnNetStart();
        }
        
    }

    private void OnNetStart()
    {
        netID = netEntity.netID;
        //Подписываемся на событие удаления данной сущности
        netEntity.OnNetDestroy += OnNetDestroy;
    }

    private void OnNetDestroy()
    {
        //Удаляем объект согласно команде сервера
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // если же мы удаляем объект по своей инициативе то сообщяем об этом сетевой сущности
        if(netEntity!=null) netEntity.Destroy();
    }
}
