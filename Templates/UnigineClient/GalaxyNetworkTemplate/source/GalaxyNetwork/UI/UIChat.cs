using System;
using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using Unigine;

namespace UnigineApp.source.GalaxyNetwork.UI
{
    public class UIChat
    {
        private UserInterface _ui;
        private Widget _contentWorld;
        private Widget _contentRoom;
        private Widget _contentSystem;
        private Gui _gui;
        private readonly GalaxyConnection _connection;

        public UIChat(GalaxyConnection connection) => _connection = connection;

        public void Show()
        {
            if (_ui != null) return;
            _gui = Gui.Get();
            if (_ui == null)
            {
                _ui = new UserInterface(_gui, "GalaxyNetwork/UI/galaxyChat.ui");
            }
            var window = _ui.GetWidget(_ui.FindWidget("window"));
            window.Arrange();
            _gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_LEFT | Gui.ALIGN_BOTTOM);
            _ui.AddCallback("send_world", Gui.CALLBACK_INDEX.CLICKED, ClickSendWorld);
            _ui.AddCallback("send_room", Gui.CALLBACK_INDEX.CLICKED, ClickSendRoom);
            _contentWorld = _ui.GetWidget(_ui.FindWidget("content_world"));
            _contentRoom = _ui.GetWidget(_ui.FindWidget("content_room"));
            _contentSystem = _ui.GetWidget(_ui.FindWidget("content_system"));

            // New incoming message event
            _connection.Api.Chat.OnChatMessage += OnChatMessage;
            // Subscribe to the internal kernel log
            GalaxyEvents.OnGalaxyLogInfo += OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarning += OnGalaxyLogWarning;
            GalaxyEvents.OnGalaxyLogError += OnGalaxyLogError;
            // Room entry event
            _connection.Events.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
        }

        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            var mess = new ChatMessage
            {
                ChannelType = ChatChannelType.Custom,
                SenderName = "Instance",
                Text = "EnterInInstance id: " + info.Id
            };
            OnChatMessage(mess);
        }

        private void OnGalaxyLogInfo(string publisher, string message)
        {
            var mess = new ChatMessage
            {
                ChannelType = ChatChannelType.Custom,
                SenderName = "Info",
                Text = publisher + " -> " + message
            };
            OnChatMessage(mess);
        }

        private void OnGalaxyLogWarning(string publisher, string message)
        {
            var mess = new ChatMessage
            {
                ChannelType = ChatChannelType.Custom,
                SenderName = "Warning",
                Text = publisher + " -> " + message
            };
            OnChatMessage(mess);
        }
        private void OnGalaxyLogError(string publisher, string message)
        {
            var mess = new ChatMessage
            {
                ChannelType = ChatChannelType.Custom,
                SenderName = "Error",
                Text = publisher + " -> " + message
            };
            OnChatMessage(mess);
        }

        private void OnChatMessage(ChatMessage message)
        {
            var content = _contentSystem;
            switch (message.ChannelType)
            {
                case ChatChannelType.World:
                    content = _contentWorld;
                    break;
                case ChatChannelType.Instance:
                    content = _contentRoom;
                    break;
            }
            AddMessage(content, message);
        }

        private void AddMessage(Widget content, ChatMessage message)
        {
            WidgetGroupBox test = new WidgetGroupBox(_gui);
            WidgetGridBox widgetGrid = new WidgetGridBox(_gui);
            widgetGrid.SetSpace(10,0);
            test.AddChild(widgetGrid);
            WidgetLabel name = new WidgetLabel(_gui, DateTime.Now.ToLongTimeString() + " " + message.SenderName)
            {
                Width = 100,
                TextAlign = 2
            };
            widgetGrid.AddChild(name);
            WidgetLabel text = new WidgetLabel(_gui, message.Text);

            widgetGrid.AddChild(text);
            content.AddChild(test);
        }




        void ClickSendWorld(Widget widget)
        {
            string text = (_ui.GetWidget(_ui.FindWidget("message_world")) as WidgetEditLine)?.Text;
            ((WidgetEditLine) _ui.GetWidget(_ui.FindWidget("message_world"))).Text = "";
            if (!string.IsNullOrWhiteSpace(text))
                _connection.Api.Chat.Send(new ChatMessage { ChannelType = ChatChannelType.World, Text = text });
        }
        void ClickSendRoom(Widget widget)
        {
            string text = (_ui.GetWidget(_ui.FindWidget("message_room")) as WidgetEditLine)?.Text;
            ((WidgetEditLine) _ui.GetWidget(_ui.FindWidget("message_room"))).Text = "";
            if (!string.IsNullOrWhiteSpace(text)) _connection.Api.Chat.SendToInstanse(text);
        }

        public void Close()
        {
            if (_ui == null) return;
            _connection.Api.Chat.OnChatMessage -= OnChatMessage;
            GalaxyEvents.OnGalaxyLogInfo -= OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarning -= OnGalaxyLogWarning;
            GalaxyEvents.OnGalaxyLogError -= OnGalaxyLogError;
            _connection.Events.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            _ui.DeleteLater();
            _ui = null;
            _contentWorld = null;
            _contentRoom = null;
            _contentSystem = null;
        }

    }
}
