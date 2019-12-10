using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetGO : MonoBehaviour
{
    [HideInInspector]
    public int localId;
    public int netID = 0;
    public bool isMy = false;
    public bool sync = false;
    private Vector3 oldPosition;
    private Quaternion oldRotation;
    private void Awake()
    {
        Invoke("Inst", 0.5f);
    }

    private void OnEnable()
    {
        //Подписываемся на сетевой тик
        GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
    }
    private void OnDisable()
    {
        GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
    }


    private void OnFrameUpdate()
    {
        //Отправляем наши координаты относительно сетевого тика.
        if (isMy) SendMyPosition();
    }

    void Inst()
    {
        if (netID == 0) StaticLinks.netGoManager.NetInstantiate(this);
    }
    void Start()
    {
        
    }
   
   
    //Отправка нашей позиции
    void SendMyPosition()
    {
        MessageTransform message = new MessageTransform();
        message.netID = netID;
        bool send = false;
        if(oldPosition!= transform.position)
        {
            message.position = transform.position.NetworkVector3();
            oldPosition = transform.position;
            send = true;
        }
        if(oldRotation != transform.rotation)
        {
            message.rotation = transform.rotation.NetworkQuaternion();
            oldRotation = transform.rotation;
            send = true;
        }        
     
       if(send) GalaxyApi.send.SendMessageToServer((byte)CommandType.goTransform, message, GalaxyCoreCommon.GalaxyDeliveryType.unreliableNewest);
    }

    private void OnDestroy()
    {
        //Сообщяем серверу об удалении сетевого объекта
        if (!isMy) return;
        MessageDestroyGO message = new MessageDestroyGO();    
        message.netID = netID;
        GalaxyApi.send.SendMessageToServer((byte)CommandType.goDestroy, message, GalaxyCoreCommon.GalaxyDeliveryType.reliable);
    }

}
