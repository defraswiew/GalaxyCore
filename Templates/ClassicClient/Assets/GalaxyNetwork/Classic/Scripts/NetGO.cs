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
        Debug.Log("Frame");
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
        message.position = transform.position.NetworkVector3();
        message.rotation = transform.rotation.NetworkQuaternion();
        GalaxyApi.send.SendMessageToServer((byte)CommandType.goTransform, message, GalaxyCoreCommon.GalaxyDeliveryType.unreliableNewest);
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
