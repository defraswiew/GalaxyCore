using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyTemplateCommon.Messages;
using GalaxyTemplateCommon;
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
        GalaxyApi.send.SendMessageToServer((byte)CommandType.worldSync, new byte[0], GalaxyCoreCommon.GalaxyDeliveryType.reliable);
    }

    private void ListenEvents(bool active)
    {
        if (active)
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect; // подписываемя на событие успешного подключения
            //Подписываемся на прослушку всех входящих сообщений
            //По хорошему сделать бы для этого отдельное собственное событие
            //Но по скольку прослушка нас интересует лишь в момент отображения UI, проблем никаких нет
            GalaxyEvents.OnGalaxyIncommingMessage += OnGalaxyIncommingMessage;
        } else
        {
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
            GalaxyEvents.OnGalaxyIncommingMessage -= OnGalaxyIncommingMessage;
        }
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
        if (code == (byte)CommandType.roomGetList)
        {
            MessageRoomInfo messageRoomInfo = MessageRoomInfo.Deserialize<MessageRoomInfo>(data);
            foreach (var item in items)
            {
                Destroy(item);
            }
            items.Clear();
            contentSize.y = 100;
            content.sizeDelta = contentSize;
            foreach (var item in messageRoomInfo.rooms)
            {
                RoomManagerItem newItem = Instantiate(prefItem, content);
                newItem.SetInfo(item.id, item.name, item.clients, item.maxClients);
                items.Add(newItem.gameObject);
                contentSize.y += 100;
                content.sizeDelta = contentSize;
            }
            count.text = messageRoomInfo.rooms.Count.ToString();
            return;
        }
        if (code == (byte)CommandType.roomEnter)
        {
            UICreate.SetActive(false);
            UI.SetActive(false);
            Active(false);
            SceneManager.LoadScene("TestLevel");
        }
    }

    private void OnGalaxyConnect(byte[] message)
    {
        SetListWindow();
        GetRoomList();
    }

    public void GetRoomList()
    {
        //Запрашиваем список комнат
        GalaxyApi.send.SendMessageToServer((byte)CommandType.roomGetList, new byte[0],GalaxyCoreCommon.GalaxyDeliveryType.reliable);
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
        MessageInstanceCreate message = new MessageInstanceCreate();
        message.name = roomName.text;
        message.maxClients = (uint)System.Int32.Parse(maxCount.text);
        GalaxyApi.send.SendMessageToServer((byte)CommandType.roomCreate, message, GalaxyCoreCommon.GalaxyDeliveryType.reliable);        
    }
    
}
