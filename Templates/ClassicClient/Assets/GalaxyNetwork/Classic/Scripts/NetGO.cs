using GalaxyCoreLib;
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
        Invoke("Inst", 1);
    }
    
    void Inst()
    {
        if (netID == 0) StaticLinks.netGoManager.NetInstantiate(this);
    }
    void Start()
    {
        
    }
   
    void FixedUpdate()
    {
        if (isMy) SendMyPosition();
    }

    void SendMyPosition()
    {
        MessageTransform message = new MessageTransform();
        message.netID = netID;
        message.position = transform.position.NetworkVector3();
        message.rotation = transform.rotation.NetworkQuaternion();
        GalaxyApi.send.SendMessageToServer((byte)CommandType.goTransform, message, GalaxyCoreCommon.GalaxyDeliveryType.unreliableNewest);
    }

    
}
