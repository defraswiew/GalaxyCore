using System.Collections.Generic;
using System.Linq;
using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using UnityEngine;
using UnityEngine.UI;


namespace GalaxyNetwork.Core.Scripts.UI
{
    public class GalaxyUIChatSystem : MonoBehaviour
    {
        [SerializeField] private int _messageLimit = 50;
        [SerializeField] private GameObject _window;
        [SerializeField] private Transform _channelContainer;
        [SerializeField] private Transform _messageContainer;
        [SerializeField] private InputField _field;
        [SerializeField] private Button _submit;
        [SerializeField] private Button _close;
        [SerializeField] private GalaxyUIChatChannel _channelPrefab;
        [SerializeField] private GalaxyUIChatMessage _messagePrefab;


        private readonly List<GalaxyUIChatChannel> _channels = new List<GalaxyUIChatChannel>();
        private GalaxyUIChatChannelModel _currentChanel;
        private readonly Queue<GalaxyUIChatMessage> _messages = new Queue<GalaxyUIChatMessage>();


        private void OnEnable()
        {
            // событие входящего сообщения чата
            GalaxyApi.Chat.OnChatMessage += OnChatMessage;
            // событие успешного подключения
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            _submit.onClick.AddListener(OnSubmit);
            _close.onClick.AddListener(OnClose);
        }

        private void OnClose()
        {
            _window.SetActive(false);
        }

        private void OnDisable()
        {
            GalaxyApi.Chat.OnChatMessage -= OnChatMessage;
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
            _submit.onClick.RemoveListener(OnSubmit);
            _close.onClick.RemoveListener(OnClose);
        }

        private void OnSubmit()
        {
            var text = _field.text;
            if (text.Length < 2) return;
            if (_currentChanel == null) return;
            var message = new ChatMessage
            {
                Text = text,
                ChannelId = _currentChanel.ChannelId,
                ChannelType = _currentChanel.ChatChannelType
            };
            _field.text = "";
            GalaxyApi.Chat.Send(message);
        }

        private void OnGalaxyConnect(byte[] message)
        {
            if (_channels.Any(x => x.Model.ChatChannelType == ChatChannelType.World))
                return;

            var model = new GalaxyUIChatChannelModel
            {
                Name = "World",
                OnClick = ChangeChannel,
                ChatChannelType = ChatChannelType.World
            };
            AddChannel(model);
        }

        private GalaxyUIChatChannel AddChannel(GalaxyUIChatChannelModel model)
        {
            var channel = Instantiate(_channelPrefab, _channelContainer);
            channel.SetData(model);
            _channels.Add(channel);
            if (_currentChanel == null) _currentChanel = channel.Model;
            return channel;
        }


        private void ChangeChannel(GalaxyUIChatChannelModel model)
        {
            Debug.Log("Change channel to " + model.Name);
            _currentChanel = model;
            ClearMessages();
            foreach (var message in _currentChanel.Messages.GetAll())
            {
                DrawMessage(message);
            }

            foreach (var channel in _channels)
            {
                if (channel.Model != _currentChanel) channel.Model.UnSelected?.Invoke();
            }
        }

        private void ClearMessages()
        {
            while (true)
            {
                if (_messages.Count == 0) return;
                var message = _messages.Dequeue();
                Destroy(message.gameObject);
            }
        }

        private void OnChatMessage(ChatMessage message)
        {
            var channel = _channels.FirstOrDefault(x => x.Model.ChannelId == message.ChannelId);
            if (channel == null)
            {
                channel = _channels.FirstOrDefault(x =>
                    x.Model.ChannelId == 0 && x.Model.ChatChannelType == message.ChannelType);
            }

            if (channel != null && message.ChannelInfo != null)
            {
                if (message.ChannelInfo.MetaCode == ChatMetaCode.Close)
                {
                    RemoveChannel(message.ChannelId);
                    return;
                }
            }

            if (channel == null)
            {
                var model = new GalaxyUIChatChannelModel
                {
                    Name = message.ChannelType + " " + message.ChannelId,
                    OnClick = ChangeChannel,
                    ChatChannelType = message.ChannelType,
                    ChannelId = message.ChannelId
                };
                if (message.ChannelInfo != null && string.IsNullOrEmpty(message.ChannelInfo.ChannelName))
                {
                    model.Name = message.ChannelInfo.ChannelName;
                }
                if (message.ChannelType == ChatChannelType.Private)
                {
                    if (message.ChannelInfo != null)
                    {
                        model.Opponent = message.ChannelInfo.PrivateMembersIds[0] != GalaxyApi.MyId
                            ? message.ChannelInfo.PrivateMembersIds[0]
                            : message.ChannelInfo.PrivateMembersIds[1];
                        model.Name = message.ChannelInfo.PrivateMembersIds[0] != GalaxyApi.MyId
                            ? message.ChannelInfo.PrivateMembers[0]
                            : message.ChannelInfo.PrivateMembers[1];
                    }
                }

                channel = AddChannel(model);
            }

            if (_currentChanel == channel.Model)
            {
                DrawMessage(message);
                channel.Model.Messages.Enqueue(message);
            }
            else
            {
                channel.Model.AddUnreadMessage(message);
            }
        }

        private void DrawMessage(ChatMessage message)
        {
            var item = Instantiate(_messagePrefab, _messageContainer);
            item.SetMessage(message, OnClickMessage);
            if (_messages.Count > _messageLimit)
            {
                var oldItem = _messages.Dequeue();
                Destroy(oldItem.gameObject);
            }

            _messages.Enqueue(item);
        }

        private void OnClickMessage(ChatMessage message)
        {
            if (GalaxyApi.MyId == message.Sender) return;
            var channel = _channels.FirstOrDefault(x => x.Model.Opponent == message.Sender);
            if (channel != null) return;
            var addChannel = new ChatMessage
            {
                ChannelType = ChatChannelType.Private,
                Recipient = message.Sender
            };

            GalaxyApi.Chat.Send(addChannel);
        }

        private void RemoveChannel(int channelId)
        {
            var channel = _channels.FirstOrDefault(x => x.Model.ChannelId == channelId);
            if (channel == null) return;
            var isCurrent = channel.Model.ChannelId == _currentChanel.ChannelId;
            _channels.Remove(channel);
            Destroy(channel.gameObject);

            if (isCurrent)
            {
                var firstChannel = _channels.FirstOrDefault();
                if (firstChannel != null)
                {
                    ChangeChannel(firstChannel.Model);
                }
            }
        }
    }
}