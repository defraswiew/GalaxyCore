using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyCoreLib
{
    [Serializable]
    /// <summary>
    /// Пример окошка создания комнаты
    /// </summary>
    public class GalaxyUIRoomCreate : MonoBehaviour
    {
        /// <summary>
        /// Статическая ссылка на самого себя
        /// </summary>
        public static GalaxyUIRoomCreate api;
        #region UI_Links
        /// <summary>
        /// Поле имени комнаты
        /// </summary>
        [SerializeField]
        private InputField roomName;
        /// <summary>
        /// Число клиентов
        /// </summary>
        [SerializeField]
        private InputField clients;
        /// <summary>
        /// Пароль на комнату
        /// </summary>
        [SerializeField]
        private InputField password;
        /// <summary>
        /// Текущий статус
        /// </summary>
        [SerializeField]
        private Text status;
        /// <summary>
        /// заданное имя команты
        /// </summary>
        [SerializeField]
        private Text roomInfoName;
        /// <summary>
        /// Описание
        /// </summary>
        [SerializeField]
        private Text roomDescription;
        /// <summary>
        /// объект прогресса
        /// </summary>
        [SerializeField]
        private GameObject progress;
        /// <summary>
        /// Ведущее окно с комнатами
        /// </summary>
        [SerializeField]
        private GalaxyUIRoom galaxyUIRoom;     
        /// <summary>
        /// контент куда кладем информацию
        /// </summary>
        [SerializeField]
        private RectTransform content;
        /// <summary>
        /// Префаб итема комнаты для UI
        /// </summary>
        [SerializeField]
        private GalaxyUiRoomInfo itemPref;  
        /// <summary>
        /// видима ли комната (кусочек тогла)
        /// </summary>
        [SerializeField]
        private GameObject btnVisible;
        /// <summary>
        /// видима ли комната (кусочек тогла)
        /// </summary>
        [SerializeField]
        private GameObject btnUnVisible;
        #endregion
        /// <summary>
        /// текущий список отображемых комнат в ui
        /// </summary>
        private List<GalaxyUiRoomInfo> items = new List<GalaxyUiRoomInfo>();
        void Awake()
        {
            if (api != null)
            {
                Debug.LogWarning("Похоже на сцене двое GalaxyUIRoomCreate");
                Destroy(gameObject);
                return;
            }
            // устанавливаем ссылку на себя
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
        /// <summary>
        /// Создаем новую комнату
        /// </summary>
        public void Create()
        {
            int clientCount = 0;
            // добываем число клиентов из строки
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
            // передаем ведущему окну запрос на создание комнаты
            galaxyUIRoom.Create(roomName.text, clientCount, password.text);
        }

    }
}
