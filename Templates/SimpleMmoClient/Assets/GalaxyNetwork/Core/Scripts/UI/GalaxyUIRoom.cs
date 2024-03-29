﻿using GalaxyCoreCommon;
using System.Collections.Generic;
using System.Linq;
using GalaxyNetwork.Core.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyCoreLib
{
    /// <summary>
    /// Пример окошка работы с инстансами (комнаты)
    /// </summary>
    public class GalaxyUIRoom : MonoBehaviour
    {
        /// <summary>
        /// Какую сцену загружать при выходе из комнаты
        /// может быть равным "" тогда ничего грузиться не будет
        /// </summary>
        [Header("Какую сцену загружать при выходе из комнаты")]      
        public string mainScene;
        /// <summary>
        /// Список комнат которые можно создавать
        /// </summary>
        [Header("Комнаты")]
        public GalaxyInstanceInfoScriptable[] instancesInfo;
        /// <summary>
        /// Текущая комната
        /// </summary>
        internal GalaxyInstanceInfoScriptable currentInfo;
        /// <summary>
        /// Окошко списка комнат
        /// </summary>
        [Header("Окна")]       
        [SerializeField]
        private GameObject roomList;
        /// <summary>
        /// Окошко создания инстанса
        /// </summary>
        [SerializeField]
        private GameObject roomCreate;
        /// <summary>
        /// Выход
        /// </summary>
        [Header("Кнопки")]
        [SerializeField]
        private GameObject exitBtn;
        /// <summary>
        /// создание комнаты
        /// </summary>
        [SerializeField]
        private GameObject createBtn;
        /// <summary>
        /// префаб строки для списка комнат
        /// </summary>
        [SerializeField]
        private GalaxyUiRoomItem itemPref;
        /// <summary>
        /// Контент скроллера
        /// </summary>
        [SerializeField]
        private RectTransform content;
        /// <summary>
        /// текущий список бъектов
        /// </summary>
        private List<GameObject> items = new List<GameObject>();      
        private bool visible = true;
        private GalaxyConnection _connection;
        void OnEnable()
        {
            _connection = GalaxyNetworkController.Api.MainConnection;
            // событие успешного подключения
            _connection.Events.OnGalaxyConnect += OnGalaxyConnect;
            // сервер прислал новый список инстансов
            _connection.Events.OnGalaxyInstancesList += OnGalaxyInstancesList;
            // мы вошли в инстанс
            _connection.Events.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            // мы вышли из инстанса
            _connection.Events.OnExitInstance += OnExitInstance;
           
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log("OnSceneLoaded");         
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _connection.Api.Instances.SyncInstance();
        }
      
        void OnDisable()
        {
            _connection.Events.OnGalaxyConnect -= OnGalaxyConnect;
            _connection.Events.OnGalaxyInstancesList -= OnGalaxyInstancesList;
            _connection.Events.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            _connection.Events.OnExitInstance -= OnExitInstance;
          
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
            Debug.Log("Вход в " + info.Name);           
            roomList.SetActive(false);
            exitBtn.SetActive(true);
            createBtn.SetActive(false);
            roomCreate.SetActive(false);
            //смотрим была ли инфа об инстансе в  GalaxyInstanceInfoScriptable 
            GalaxyInstanceInfoScriptable target = instancesInfo.FirstOrDefault(x => x.type == info.Type);
            if (target == null)
            {
                _connection.Api.Instances.SyncInstance();
            } else
            {
                if (target.sceneName != "")
                {
                    SceneManager.LoadSceneAsync(target.sceneName);
                    SceneManager.sceneLoaded += OnSceneLoaded;                
                } else
                {
                    _connection.Api.Instances.SyncInstance();
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
                GalaxyInstanceInfoScriptable target = instancesInfo.FirstOrDefault(x => x.type == item.Type);
                Sprite sprite = null;
                if (target != null) sprite = target.img;
                GalaxyUiRoomItem newItem = Instantiate(itemPref, content);
                newItem.Init(item.Id, item.Name, item.Clients, item.MaxClients, !item.Password, sprite);
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
            _connection.Api.Instances.Create(name, count,currentInfo.type, password,visible);
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
            _connection.Api.Instances.InstanceList();
            
        }
        /// <summary>
        /// Выход из инстанса
        /// </summary>
        public void ExitInstance()
        {
            _connection.Api.Instances.ExitInstance();
            Clear();
            GetRoomList();
        }

        public void DrowRoomList()
        {
            roomList.SetActive(true);
            GetRoomList();
            if (_connection.Api.Instances.Current == null)
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
