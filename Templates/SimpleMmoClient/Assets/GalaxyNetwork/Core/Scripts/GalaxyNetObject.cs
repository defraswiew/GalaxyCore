using GalaxyCoreCommon.NetEntity;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyNetObject : UnityNetEntity, INetEntity
{
   
    private int localId;   
    public bool syncPosition = true;
    public bool syncRotation = true;
    private UnityNetObject netEntity;
    private void OnEnable()
    {
        Debug.Log("OnEnable GalaxyNetObject");
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
