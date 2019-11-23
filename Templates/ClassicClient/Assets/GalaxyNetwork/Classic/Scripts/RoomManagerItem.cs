using GalaxyCoreLib;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomManagerItem : MonoBehaviour
{
    public Text id;
    public Text roomName;
    public Text clients;
    private int myId;
    
    public void SetInfo(int id, string roomName, uint clients, uint maxClients)
    {
        myId = id;
        this.id.text = id.ToString();
        this.roomName.text = roomName;
        this.clients.text = "Онлайн: " + clients + ",  Максимум: " + maxClients;
    }

    public void ConnectRoom()
    {
        MessageRoomEnter message = new MessageRoomEnter();
        message.id = myId;
        GalaxyApi.send.SendMessageToServer((byte)CommandType.roomEnter, message, GalaxyCoreCommon.GalaxyDeliveryType.reliable);
    }
    
}
