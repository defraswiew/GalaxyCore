using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System;
using System.Collections.Generic;
using System.Text;
using Unigine;

namespace UnigineApp.GalaxyNetwork
{
    public class UIChat
    {
        public UserInterface ui;
        Widget contentWorld;
        Widget contentRoom;
        Widget contentSystem;
        Gui gui;
 
        public void Drow()
        {
            gui = Gui.Get();
            if (ui == null) ui = new UserInterface(gui, "GalaxyNetwork/UI/galaxyChat.ui");            
            Widget window = ui.GetWidget(ui.FindWidget("window"));           
            window.Arrange();
            gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_LEFT | Gui.ALIGN_BOTTOM);
            ui.AddCallback("send_world", Gui.CALLBACK_INDEX.CLICKED, ClickSendWorld);
            ui.AddCallback("send_room", Gui.CALLBACK_INDEX.CLICKED, ClickSendRoom);
            contentWorld = ui.GetWidget(ui.FindWidget("content_world"));
            contentRoom = ui.GetWidget(ui.FindWidget("content_room"));
            contentSystem = ui.GetWidget(ui.FindWidget("content_system"));

            // New incoming message event
            // Событие нового вдодящего сообщения
            GalaxyApi.chat.OnChatMessage += OnChatMessage;
            // Subscribe to the internal kernel log
            // События логов ядра
            GalaxyEvents.OnGalaxyLogInfo += OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarnig += OnGalaxyLogWarnig;
            GalaxyEvents.OnGalaxyLogError += OnGalaxyLogError;
            // Room entry event
            // Событие входа в комнату
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
        }

        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            MessChat mess = new MessChat();
            mess.Type = GalaxyCoreCommon.ChatMessageType.none;
            mess.name = "Instance";
            mess.text = "EnterInInstance id: " + info.id;
            OnChatMessage(mess);
        }

        private void OnGalaxyLogInfo(string publisher, string message)
        {
            MessChat mess = new MessChat();
            mess.Type = GalaxyCoreCommon.ChatMessageType.none;
            mess.name = "Info";
            mess.text = publisher + " -> "+ message;
            OnChatMessage(mess);
        }
        private void OnGalaxyLogWarnig(string publisher, string message)
        {
            MessChat mess = new MessChat();
            mess.Type = GalaxyCoreCommon.ChatMessageType.none;
            mess.name = "Warnig";
            mess.text = publisher + " -> " + message;
            OnChatMessage(mess);
        }
        private void OnGalaxyLogError(string publisher, string message)
        {
            MessChat mess = new MessChat();
            mess.Type = GalaxyCoreCommon.ChatMessageType.none;
            mess.name = "Error";
            mess.text = publisher + " -> " + message;
            OnChatMessage(mess);
        }

        private void OnChatMessage(MessChat message)
        {
            Widget content = contentSystem;
            switch (message.Type)
            {             
                case GalaxyCoreCommon.ChatMessageType.all:
                    content = contentWorld;
                    break;              
                case GalaxyCoreCommon.ChatMessageType.instance:
                    content = contentRoom;
                    break; 
            }
            AddMessage(content, message);
        }

        private void AddMessage(Widget content, MessChat message)
        {
            WidgetGroupBox test = new WidgetGroupBox(gui);          
            WidgetGridBox widgetGrid = new WidgetGridBox(gui);           
            widgetGrid.SetSpace(10,0);
            test.AddChild(widgetGrid);
            WidgetLabel name = new WidgetLabel(gui, DateTime.Now.ToLongTimeString() + " " + message.name);
            name.Width = 100;
            name.TextAlign = 2;
            widgetGrid.AddChild(name);
            WidgetLabel text = new WidgetLabel(gui, message.text);
             
            widgetGrid.AddChild(text);
            content.AddChild(test);
        }

       


        void ClickSendWorld(Widget widget)
        {
            string text = (ui.GetWidget(ui.FindWidget("message_world")) as WidgetEditLine).Text;
            (ui.GetWidget(ui.FindWidget("message_world")) as WidgetEditLine).Text = "";
            GalaxyApi.chat.SendAll(text);           
        }
        void ClickSendRoom(Widget widget)
        {
            string text = (ui.GetWidget(ui.FindWidget("message_room")) as WidgetEditLine).Text;
            (ui.GetWidget(ui.FindWidget("message_room")) as WidgetEditLine).Text = "";
            GalaxyApi.chat.SendToInstanse(text);
        }



        public void Close()
        {
            if (ui == null) return;
            GalaxyApi.chat.OnChatMessage -= OnChatMessage;
            GalaxyEvents.OnGalaxyLogInfo -= OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarnig -= OnGalaxyLogWarnig;
            GalaxyEvents.OnGalaxyLogError -= OnGalaxyLogError;
            GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            ui.DeleteLater();
        }

     
    }
}
