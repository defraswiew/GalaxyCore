using GalaxyCoreCommon;
using GalaxyCoreLib.Api;
using GalaxyCoreLib;
using SimpleMmoCommon;
using SimpleMmoCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib.NetEntity;

public class NetEntityTransform : MonoBehaviour
{
    /// <summary>
    ///  Ссылка на сетевую сущность в ядре
    /// </summary>
    ClientNetEntity netEntity;
    public bool sendMyPosition;
    public bool sendMyRotation;

    private void Awake()
    {
        netEntity = GetComponent<UnityNetEntity>().netEntity;
    }

    private void OnEnable()
    {
        GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
    }
    private void OnDisable()
    {
        GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
    }
    private void OnFrameUpdate()
    {
        if (!netEntity.isMy) return;
        if (sendMyPosition) netEntity.SendPosition(transform.position.NetworkVector3());
        if (sendMyRotation) netEntity.SendRotation(transform.rotation.NetworkQuaternion());   
    }

    public void Update()
    {
        if (!netEntity.isMy)        
        {
            transform.position = Vector3.Lerp(transform.position, netEntity.position.Vector3(), GalaxyApi.lerpDelta);
           if(!netEntity.rotation.isZero()) transform.rotation = Quaternion.Lerp(transform.rotation, netEntity.rotation.Quaternion(), GalaxyApi.lerpDelta);
        }      
    }

   


 
      
}
