using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyCoreLib
{
    public class GalaxyUIRoom : MonoBehaviour
    {
        [Header("Какую сцену загружать при выходе из комнаты")]
        /// может быть равным "" тогда ничего грузиться не будет
        public string mainScene;
        [Header("Комнаты")]
        public GalaxyInstanceInfoScriptable[] instancesInfo;
        internal GalaxyInstanceInfoScriptable currentInfo;
        [Header("Окна")]
        public GameObject roomList;
        public GameObject roomCreate;
        [Header("Кнопки")]
        public GameObject exitBtn;
        public GameObject createBtn;
        public GalaxyUiRoomItem itemPref;
        public RectTransform content;
        private List<GameObject> items = new List<GameObject>();
        private bool visible = true;
        void OnEnable()
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            GalaxyEvents.OnGalaxyInstancesList += OnGalaxyInstancesList;
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            GalaxyEvents.OnExitInstance += OnExitInstance;
           
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GalaxyApi.instances.SyncInstance();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnDisable()
        {
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
            GalaxyEvents.OnGalaxyInstancesList -= OnGalaxyInstancesList;
            GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            GalaxyEvents.OnExitInstance -= OnExitInstance;
          
        }
        /// <summary>
        /// Событие успешного коннекта
        /// </summary>
        /// <param name="message"></param>
        private void OnGalaxyConnect(byte[] message)
        {
            roomList.SetActive(true);
            GetRoomList();
        }
        /// <summary>
        /// Событие выхода из инстанса
        /// </summary>
        private void OnExitInstance()
        {
            exitBtn.SetActive(false);
            createBtn.SetActive(true);      
            if(mainScene!="") SceneManager.LoadSceneAsync(mainScene);           
        }
        /// <summary>
        /// Сервер подтвердил вход в инстанс
        /// </summary>
        /// <param name="info"></param>
        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            Debug.Log("Вход в " + info.name);           
            roomList.SetActive(false);
            exitBtn.SetActive(true);
            createBtn.SetActive(false);
            roomCreate.SetActive(false);
            //смотрим была ли инфа об инстансе в  GalaxyInstanceInfoScriptable 
            GalaxyInstanceInfoScriptable target = instancesInfo.Where(x => x.type == info.type).FirstOrDefault();
            if (target == null)
            {
                GalaxyApi.instances.SyncInstance();
            } else
            {
                if (target.sceneName != "")
                {
                    SceneManager.LoadSceneAsync(target.sceneName);
                    SceneManager.sceneLoaded += OnSceneLoaded;
                } else
                {
                    GalaxyApi.instances.SyncInstance();
                }
               
            }
            
        }
        /// <summary>
        /// Сервер вернул список инстансов
        /// </summary>
        /// <param name="instances"></param>
        private void OnGalaxyInstancesList(List<InstanceInfo> instances)
        {
            Clear();
            foreach (var item in instances)
            {
                GalaxyInstanceInfoScriptable target = instancesInfo.Where(x => x.type == item.type).FirstOrDefault();
                Sprite sprite = null;
                if (target != null) sprite = target.img;
                 GalaxyUiRoomItem newItem = Instantiate(itemPref, content);
                newItem.Init(item.id, item.name, item.clients, item.maxClients, !item.password, sprite);
                items.Add(newItem.gameObject);
            }
        }
        /// <summary>
        /// Создаем новый инстанс
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <param name="password"></param>
        public void Create(string name, int count, string password)
        {
            //пример создания инстанса с возможностью задать пароль, видимость, и тип
            GalaxyApi.instances.Create(name, count,currentInfo.type, password,visible);
        }
        /// <summary>
        /// Устанавливаем из UI видимая комната или нет
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value)
        {
            visible = value;
        }
        /// <summary>
        /// Чистим UI
        /// </summary>
        private void Clear()
        {
            foreach (var item in items)
            {
                Destroy(item);
            }
            items.Clear();
        }
        /// <summary>
        /// Запрашиваем список инстансов
        /// </summary>
        public void GetRoomList()
        {
            GalaxyApi.instances.InstanceList();
        }
        /// <summary>
        /// Выход из инстанса
        /// </summary>
        public void ExitInstance()
        {
            GalaxyApi.instances.ExitInstance();
            Clear();
            GetRoomList();
        }

        public void DrowRoomList()
        {
            roomList.SetActive(true);
            GetRoomList();
            if (GalaxyApi.instances.current == null)
            {
                exitBtn.SetActive(false);
                createBtn.SetActive(true);
            }
            else
            {
                exitBtn.SetActive(true);
                createBtn.SetActive(false);
            }
        }

    }
}
