using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleRoomCreate : MonoBehaviour
{
    public string room_name;
    public byte type;   
    public void Create()
    {
        GalaxyApi.Instances.Create(room_name, 100, type);
        GameObject.Find("ExampleRooms").SetActive(false); 
    }
}
