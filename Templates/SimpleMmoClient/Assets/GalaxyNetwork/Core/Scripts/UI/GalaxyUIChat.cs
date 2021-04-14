using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages; 
using GalaxyCoreLib.Api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyCoreLib
{
    /// <summary>
    /// Простенький пример работы с чатом
    /// В будующих версиях будет заменен
    /// </summary>
    public class GalaxyUIChat : MonoBehaviour
    {
        /// <summary>
        /// Название системного канала
        /// </summary>
        public string sysName = "Система";
        /// <summary>
        /// Название канала комнаты
        /// </summary>
        public string instanseName = "Комната";
        /// <summary>
        /// Цвет сообщений системного канала
        /// </summary>
        [SerializeField]
        private Color sysColor;
        /// <summary>
        /// Основное окно чата
        /// </summary>
        [SerializeField]
        private GameObject window;
        /// <summary>
        /// контент скрола
        /// </summary>
        [SerializeField]
        private Transform content;
        /// <summary>
        /// Поле ввода
        /// </summary>
        [SerializeField]
        private InputField field;
        /// <summary>
        /// Выбор активного канала
        /// </summary>
        [SerializeField]
        private Dropdown dropdown;
        /// <summary>
        /// До поле краткой информации
        /// </summary>
        [SerializeField]
        private Text info;
        /// <summary>
        /// Ссылка на префаб сообщения
        /// </summary>
        [SerializeField]
        private GalaxyUIChatMessage messagePref;
        /// <summary>
        /// список каналов
        /// </summary>
        private Dictionary<string, ChenallItem> chenalls = new Dictionary<string, ChenallItem>();
        /// <summary>
        /// Текущий активный канал
        /// </summary>
        private ChenallItem currentChenall;

        void OnEnable()
        {
            // событие входящего сообщения чата
            GalaxyApi.Chat.OnChatMessage += OnChatMessage;
            // событие входа в комнату
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            // кто то вошел в комнату
            GalaxyEvents.OnGalaxyIncomingClient += OnGalaxyIncomingClient;
            // событие успешного подключения
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            // смена канала
            dropdown.onValueChanged.AddListener(ChangeChenall);
            // что то напечатали в поле ввода
            field.onValueChanged.AddListener(ChangeField);
        }

        /// <summary>
        /// Вспомогательный метод добавления канала
        /// </summary>
        /// <param name="chenall"></param>
        private void AddCHenall(ChenallItem chenall)
        {
            chenalls.Add(chenall.name, chenall);
            dropdown.options.Add(new Dropdown.OptionData(chenall.name));
            field.interactable = true;
            if (chenalls.Count == 1) ChangeChenall(0);
        }

        private void ChangeField(string text)
        {
            if (currentChenall == null) field.interactable = false;
        }

        void OnDisable()
        {
            // отписываемся от слушаемых событий
            GalaxyApi.Chat.OnChatMessage -= OnChatMessage;
            GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            GalaxyEvents.OnGalaxyIncomingClient -= OnGalaxyIncomingClient;
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
            dropdown.onValueChanged.RemoveListener(ChangeChenall);
            field.onValueChanged.RemoveListener(ChangeField);
        }
        /// <summary>
        /// Смена канала по его ид
        /// </summary>
        /// <param name="id"></param>
        void ChangeChenall(int id)
        {
            currentChenall = chenalls[dropdown.options[id].text];
            info.text = "[" + currentChenall.name + "]";
        }

        private void OnGalaxyConnect(byte[] message)
        {
            Draw(false);
            ChenallItem chenall = new ChenallItem();
            chenall.name = "Мир";
            chenall.chatMessageType = ChatMessageType.all;
            AddCHenall(chenall);
        }
        public void Draw(bool visible)
        {
            window.SetActive(visible);
        }

        private void OnGalaxyIncomingClient(RemoteClient client)
        {
            // Сообщяем в системный чат о входе игрока
            SetItemInWindow("<color=#" + ColorUtility.ToHtmlStringRGB(sysColor) + "><b>[" + sysName + "]</b>: К нам присоеденился " + client.Name + "</color>");
        }

        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            // Сообщяем в системный чат о входе инстанс
            SetItemInWindow("<color=#" + ColorUtility.ToHtmlStringRGB(sysColor) + "><b>[" + sysName + "]</b>: Вход в инстанс " + info.Name + "</color>");
            if (chenalls.ContainsKey(instanseName)) return;
            // а за одно и создаем новый канал
            ChenallItem chenall = new ChenallItem();
            chenall.name = instanseName;
            chenall.chatMessageType = ChatMessageType.instance;
            AddCHenall(chenall);
        }
        /// <summary>
        /// Новое входящее сообщение
        /// </summary>
        /// <param name="message"></param>
        private void OnChatMessage(MessChat message)
        {
            // раскидываем со общения по их типу
            switch (message.Type)
            {
                case ChatMessageType.all:
                    SetItemInWindow("<b>[Мир] " + message.Name + "</b>:" + message.Text);
                    break;
                case ChatMessageType.privateMessage:
                    break;
                case ChatMessageType.instance:
                    SetItemInWindow("<b>[" + instanseName + "] " + message.Name + "</b>:" + message.Text);
                    break;
                case ChatMessageType.group:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Выводим в чат новое сообщение
        /// </summary>
        /// <param name="text"></param>
        private void SetItemInWindow(string text)
        {
            GalaxyUIChatMessage item = Instantiate(messagePref, content);
            item.SetMessage(text);
        }
        /// <summary>
        /// Отправляем сообщение
        /// </summary>
        public void Send()
        {
            if (field.text.Length < 3) return;
            switch (currentChenall.chatMessageType)
            {
                case ChatMessageType.all:
                    GalaxyApi.Chat.SendAll(field.text);
                    break;
                case ChatMessageType.privateMessage:
                    GalaxyApi.Chat.SendToClient(currentChenall.target, field.text);
                    break;
                case ChatMessageType.instance:
                    GalaxyApi.Chat.SendToInstanse(field.text);
                    break;
                case ChatMessageType.group:
                    GalaxyApi.Chat.SendGroup(currentChenall.target, field.text);
                    break;
                default:
                    break;
            }
            field.text = "";
        }

    }
    /// <summary>
    /// Краткое описание канала
    /// </summary>
    public class ChenallItem
    {
        public ChatMessageType chatMessageType;
        public string name;
        public int target;
    }
}
