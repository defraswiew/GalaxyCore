using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GalaxyUiRoomItem : MonoBehaviour
{
    public Text roomName;
    public Text userCount;
    public GameObject open;
    public GameObject close;
    public Button btn;
    private int id;
    public Image logo;
    private string password="";
    public GameObject passField;
    private bool isOpen;

    public void Init(int roomId, string nameRoom, int count, int max, bool isOpen, Sprite img = null)
    {
        id = roomId;
        roomName.text = nameRoom;
        userCount.text = count + " /" + max;
        this.isOpen = isOpen;
        if (isOpen)
        {
            open.SetActive(true);
        } else
        {
            close.SetActive(true);
        }
        if (img != null) logo.sprite = img;
    }

    public void SetPassword(string password)
    {
        this.password = password;
    }

    public void Connect()
    {
        Debug.Log(1);
        if (!isOpen && password=="")
        {
            Debug.Log(2);
            passField.SetActive(true);
            return;
        }
        Debug.Log(3);
        GalaxyApi.instances.EnterToInstance(id,password);
    }
}
