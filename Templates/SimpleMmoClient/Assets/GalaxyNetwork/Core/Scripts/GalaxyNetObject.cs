using GalaxyCoreCommon.NetEntity;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyNetObject : MonoBehaviour , INetEntity
{
    public int netId = 0;
    private int localId;
    public bool isMy = false;
    public bool syncPosition = true;
    public bool syncRotation = true;
    private UnityNetObject netEntity;
    private void Start()
    {
     
        if (netId == 0)
        {
            // если netId все еще 0 то это явно наш объект
            // и нужно отправлять запрос на создание сетевого экземпляра
            // создаем пустой экземпляр сетевой сущности
            netEntity = new UnityNetObject();          
            // убираем все лишнее из имени
            netEntity.name = gameObject.name.Split(new char[] { ' ', '(' })[0];
            // отправляем запрос на создание сетевого объекта
            GalaxyApi.netEntity.Instantiate(netEntity);
        } else
        {
            // если к моменту вызова start ид уже есть, значит это точно не наш объект
            isMy = false;
        }
        AddInterfaces();
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

    /// <summary>
    /// Вызывается при полной сетевой инициализации объекта
    /// </summary>
    public void NetStart()
    {
        netId = netEntity.netID;
        isMy = (netEntity.owner == GalaxyApi.myId);        
    }
}
