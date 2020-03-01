using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{  
    public GameObject UI;
    public GameObject UICreate;
    public GameObject UISmall;
    public Button createButton;
    public RoomManagerItem prefItem;
    public RectTransform content;
    public Text count;
    public InputField roomName;
    public InputField maxCount;
    private Vector2 contentSize;
    private List<GameObject> items = new List<GameObject>();
    private void OnEnable()
    {
        ListenEvents(true);
        SceneManager.sceneLoaded += SceneLoaded; 
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Загрузилась новая сцена, запросим у сервера её состояние
        if (!GalaxyApi.connection.isConnected) return;
      //  GalaxyApi.send.SendMessageToServer((byte)CommandType.worldSync, new byte[0], GalaxyCoreCommon.GalaxyDeliveryType.reliable);
    }

    private void ListenEvents(bool active)
    {
        if (active)
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            GalaxyEvents.OnGalaxyInstancesList += OnGalaxyInstancesList;
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
        } else
        {
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
            GalaxyEvents.OnGalaxyInstancesList -= OnGalaxyInstancesList;
            GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
        }
    }

    private void OnGalaxyEnterInInstance(GalaxyCoreCommon.InternalMessages.InstanceInfo info)
    {
        Debug.Log("Вход в " + info.name);
        UICreate.SetActive(false);
        UI.SetActive(false);
        Active(false);
        //  SceneManager.LoadScene("TestLevel");
        GalaxyApi.instances.SyncInstance();
    }

    private void OnGalaxyInstancesList(List<GalaxyCoreCommon.InternalMessages.InstanceInfo> instances)
    {
        foreach (var item in items)
        {
            Destroy(item);
        }
        items.Clear();
        contentSize.y = 100;
        content.sizeDelta = contentSize;
        foreach (var item in instances)
        {
            RoomManagerItem newItem = Instantiate(prefItem, content);
            newItem.SetInfo(item.id, item.name, item.clients, item.maxClients);
            items.Add(newItem.gameObject);
            contentSize.y += 100;
            content.sizeDelta = contentSize;
        }
        count.text = instances.Count.ToString();
    }

    private void OnDisable()
    {
        ListenEvents(false);
    }

    public void SetCreateWindow()
    {
        UICreate.SetActive(true);
        UI.SetActive(false);
        createButton.interactable = true;
    }

    public void SetListWindow()
    {
        UICreate.SetActive(false);
        UI.SetActive(true);    
    }

    private void OnGalaxyIncommingMessage(byte code, byte[] data)
    {
        
    }

    private void OnGalaxyConnect(byte[] message)
    {
        SetListWindow();
        GetRoomList();
    }

    public void GetRoomList()
    {
        // запрашиваем список инстансов
        GalaxyApi.instances.InstanceList();
    }
    

    public void Active(bool active)
    {
        if (active)
        {
            SetListWindow();
            UISmall.SetActive(false);
            GetRoomList();
        } else
        {
            UICreate.SetActive(false);
            UI.SetActive(false);
            UISmall.SetActive(true);
        }
        ListenEvents(active);
    }


   public void CreateRoom()
    {       
        createButton.interactable = false;      
        GalaxyApi.instances.Create(roomName.text, (int)System.Int32.Parse(maxCount.text));
       
    }
    


}
