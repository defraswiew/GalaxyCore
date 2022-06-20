using GalaxyNetwork.Core.Scripts;
using UnityEngine;

public class ExampleRoomCreate : MonoBehaviour
{
    public string room_name;
    public byte type;   
    
    public void Create()
    {
        GalaxyNetworkController.Api.MainConnection.Api.Instances.Create(room_name, 100, type);
        GameObject.Find("ExampleRooms").SetActive(false); 
    }
}
