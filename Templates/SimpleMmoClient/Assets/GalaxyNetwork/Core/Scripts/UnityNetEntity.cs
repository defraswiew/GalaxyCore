using GalaxyCoreCommon.NetEntity;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityNetEntity : MonoBehaviour
{
    public int netId = 0;  
    public bool isMy = false;
    public bool syncPosition;
    public bool syncRotation;
    public bool syncScale;
    private UnityNetObject netEntity;
     

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    void Start()
    {
        // инициализация объекта
        Init();  
        // подписивыемся на подтверждение сервером локальной инициализации
       GalaxyApi.netEntity.OnGalaxyLocalInstantiate += OnGalaxyLocalInstantiate;
    }

    /// <summary>
    /// Вызывается сервером при успешном создании нашего объекта
    /// </summary>
    /// <param name="entity"></param>
    private void OnGalaxyLocalInstantiate(GalaxyNetEntity entity)
    {
        // по идее этот метод не может быть вызван если это не наш объект, но все равно проверяем
        isMy = (entity.owner == GalaxyApi.myId);
        netId = entity.netID;            
        // подписываемся на сетевой тик
        if(isMy) GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
    }

    private void OnFrameUpdate()
    {
        if (!isMy) return;
       ((NetworkTransformUnity)netEntity.transform).SyncTransform(syncPosition, syncRotation, syncScale);
    }

    private void OnDisable()
    {
        GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
    }

    private void SyncTransform(GalaxyTransform netTransform)
    {
        if (netTransform.position != null)
        {
           transform.position = netTransform.position.Vector3();         
        }
        if (netTransform.rotation != null)
        {
          transform.rotation = netTransform.rotation.Quaternion();          
        }
    }    

    void Init()
    {
         
        if (netId == 0)
        {
            // если netId все еще 0 то это явно наш объект
            // и нужно отправлять запрос на создание сетевого экземпляра
            // создаем пустой экземпляр сетевой сущности
            netEntity = new UnityNetObject();
            // убираем все лишнее из имени
            netEntity.name = gameObject.name.Split(new char[] { ' ', '(' })[0];
            if (syncPosition || syncRotation) netEntity.transform = new NetworkTransformUnity(transform, netEntity);
             
            // отправляем запрос на создание сетевого объекта
            GalaxyApi.netEntity.Instantiate(netEntity);
        }
        else
        {
            // если к моменту вызова start ид уже есть, значит это точно не наш объект
            isMy = false;
        }
    //    AddInterfaces();
      //  yield return null;
    }


    /// <summary>
    /// Собираем все сетевые интерфейсы на объекте
    /// </summary>
    private void AddInterfaces()
    {
        foreach (var item in gameObject.GetComponents<INetEntity>())
        {
            netEntity.interfaces.Add(item);
        }
    }
}
