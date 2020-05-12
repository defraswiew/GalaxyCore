using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GalaxyCoreLib
{
    public class GalaxyUIRoomCreate : MonoBehaviour
    {
        public InputField roomName;
        public InputField clients;
        public InputField password;
        public Text status;
        public Text roomInfoName;
        public Text roomDescription;
        public GameObject progress;
        public GalaxyUIRoom galaxyUIRoom;
        public static GalaxyUIRoomCreate api;      
        public RectTransform content;
        public GalaxyUiRoomInfo itemPref;
        private List<GalaxyUiRoomInfo> items = new List<GalaxyUiRoomInfo>();
        public  GameObject btnVisible;
        public GameObject btnUnVisible;
        void Awake()
        {
            api = this;
        }
        void OnEnable()
        {
            progress.SetActive(false);
            status.text = "";
            Clear();
            foreach (var item in galaxyUIRoom.instancesInfo)
            {
                GalaxyUiRoomInfo newItem = Instantiate(itemPref, content);
                newItem.Init(item);
                items.Add(newItem);
            }
            if (items.Count > 0)
            {
                items[0].Click();
            }
            btnVisible.SetActive(true);
            btnUnVisible.SetActive(false);
            galaxyUIRoom.SetVisible(true);
        }

        void Clear()
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
        }

        public void SelectedInfo(GalaxyInstanceInfoScriptable info)
        {
            galaxyUIRoom.currentInfo = info;
            roomInfoName.text = info.uiName;
            roomDescription.text = info.uiDescription;
        }

        public void Create()
        {
            int clientCount = 0;

            System.Int32.TryParse(clients.text, out clientCount);
            if (roomName.text.Length < 3)
            {
                status.text = "Короткое имя";
                return;
            }
            if (!GalaxyApi.connection.isConnected)
            {
                status.text = "Нет подключения к серверу";
                return;
            }
            status.text = "Создаю";
            progress.SetActive(true);
            galaxyUIRoom.Create(roomName.text, clientCount, password.text);
        }

    }
}
