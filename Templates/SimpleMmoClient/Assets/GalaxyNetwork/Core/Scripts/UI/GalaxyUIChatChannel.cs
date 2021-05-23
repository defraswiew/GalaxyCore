using System;
using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyNetwork.Core.Scripts.UI
{
    public class GalaxyUIChatChannelModel
    {
        public int ChannelId;
        public string Name;
        public Action<GalaxyUIChatChannelModel> OnClick;
        public ChatChannelType ChatChannelType;
        public int Opponent;
        public LimitedQueue<ChatMessage> Messages = new LimitedQueue<ChatMessage>(50);
        public Action UnSelected;
        public Action OnUnreadMessage;
        
        public void AddUnreadMessage(ChatMessage message)
        {
            Messages.Enqueue(message);
            OnUnreadMessage?.Invoke();
        }
    }
    public class GalaxyUIChatChannel : MonoBehaviour
    {
        [SerializeField]
        private Text _header;
        [SerializeField]
        private Text _count;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private GameObject _selected;

        private int _unreadMessages;
    

        public GalaxyUIChatChannelModel Model;

        public void SetData(GalaxyUIChatChannelModel model)
        {
            _header.text = model.Name;
            Model = model;
            _button.onClick.AddListener(() =>
            {
                Model.OnClick?.Invoke(Model); 
                _selected.SetActive(true);
                _unreadMessages = 0;
                _count.text = "";
            });
            Model.UnSelected += UnSelected;
            Model.OnUnreadMessage += OnUnreadMessage;
        }

        private void OnDestroy()
        {
            Model.UnSelected -= UnSelected;
        }

        private void OnUnreadMessage()
        {
            _unreadMessages++;
            _count.text = _unreadMessages.ToString();
        }

        private void UnSelected()
        {
            _selected.SetActive(false);
        }
    }
}
