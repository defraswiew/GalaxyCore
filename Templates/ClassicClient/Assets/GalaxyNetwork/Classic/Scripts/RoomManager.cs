using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyTemplateCommon.Messages;
using GalaxyTemplateCommon;

public class RoomManager : MonoBehaviour
{
    
    void Start()
    {
        InvokeRepeating("CreateRoom",5f, 5f);
    }


    void CreateRoom()
    {
        MessageInstanceCreate message = new MessageInstanceCreate();
        message.name = "Тест" + Time.time;
        message.maxClients = (uint)Random.Range(5, 65000);
        GalaxyApi.send.SendMessageToServer((byte)CommandType.roomCreate, message, GalaxyCoreCommon.GalaxyDeliveryType.reliable);
    }
    
}
