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
 
        public void Show()
        {
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
            GalaxyApi.Chat.OnChatMessage += OnChatMessage;
            // Subscribe to the internal kernel log
            GalaxyEvents.OnGalaxyLogInfo += OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarning += OnGalaxyLogWarning;
            GalaxyEvents.OnGalaxyLogError += OnGalaxyLogError;
            // Room entry event
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
        }

        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            var mess = new MessChat
            {
                Type = ChatMessageType.none,
                Name = "Instance",
                Text = "EnterInInstance id: " + info.Id
            };
            OnChatMessage(mess);
        }

        private void OnGalaxyLogInfo(string publisher, string message)
        {
            var mess = new MessChat
            {
                Type = ChatMessageType.none, 
                Name = "Info", 
                Text = publisher + " -> " + message
            };
            OnChatMessage(mess);
        }
        
        private void OnGalaxyLogWarning(string publisher, string message)
        {
            var mess = new MessChat
            {
                Type = ChatMessageType.none, 
                Name = "Warning", 
                Text = publisher + " -> " + message
            };
            OnChatMessage(mess);
        }
        private void OnGalaxyLogError(string publisher, string message)
        {
            var mess = new MessChat
            {
                Type = ChatMessageType.none, 
                Name = "Error", 
                Text = publisher + " -> " + message
            };
            OnChatMessage(mess);
        }

        private void OnChatMessage(MessChat message)
        {
            var content = _contentSystem;
            switch (message.Type)
            {             
                case ChatMessageType.all:
                    content = _contentWorld;
                    break;              
                case ChatMessageType.instance:
                    content = _contentRoom;
                    break; 
            }
            AddMessage(content, message);
        }

        private void AddMessage(Widget content, MessChat message)
        {
            WidgetGroupBox test = new WidgetGroupBox(_gui);          
            WidgetGridBox widgetGrid = new WidgetGridBox(_gui);           
            widgetGrid.SetSpace(10,0);
            test.AddChild(widgetGrid);
            WidgetLabel name = new WidgetLabel(_gui, DateTime.Now.ToLongTimeString() + " " + message.Name)
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
            GalaxyApi.Chat.SendAll(text);           
        }
        void ClickSendRoom(Widget widget)
        {
            string text = (_ui.GetWidget(_ui.FindWidget("message_room")) as WidgetEditLine)?.Text;
            ((WidgetEditLine) _ui.GetWidget(_ui.FindWidget("message_room"))).Text = "";
            GalaxyApi.Chat.SendToInstanse(text);
        }

        public void Close()
        {
            if (_ui == null) return;
            GalaxyApi.Chat.OnChatMessage -= OnChatMessage;
            GalaxyEvents.OnGalaxyLogInfo -= OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarning -= OnGalaxyLogWarning;
            GalaxyEvents.OnGalaxyLogError -= OnGalaxyLogError;
            GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            _ui.DeleteLater();
        }

    }
}