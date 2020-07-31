using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System;
using System.Collections.Generic;
using System.Text;
using Unigine;

namespace UnigineApp.GalaxyNetwork
{
   public class UIInstances
    {
        public UserInterface ui;
        Widget content;
        Gui gui;
        List<Widget> items;
        public void Drow()
        {
            gui = Gui.Get();
            if (ui == null) ui = new UserInterface(gui, "GalaxyNetwork/UI/galaxyInstances.ui"); 
            Widget window = ui.GetWidget(ui.FindWidget("window"));
            window.Arrange();
            gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
            ui.AddCallback("update", Gui.CALLBACK_INDEX.CLICKED, ClickUpdate);
            ui.AddCallback("create", Gui.CALLBACK_INDEX.CLICKED, ClickCreate);
            content = ui.GetWidget(ui.FindWidget("content"));
            items = new List<Widget>();

            // Instance List Update Event
            // Пришел ответ на запрос списка инстансов
            GalaxyEvents.OnGalaxyInstancesList += OnGalaxyInstancesList;
            // Requesting an up - to - date list of instances
            // Запрашиваем список инстансов
            if (GalaxyApi.connection.isConnected) GalaxyApi.instances.InstanceList();
        }

        private void OnGalaxyInstancesList(List<InstanceInfo> instances)
        {             
            foreach (var item in items)
            {
                content.RemoveChild(item);
            }
            items.Clear();
            foreach (var item in instances)
            {
                AddItem(item);
            }
        }


        void ClickUpdate(Widget widget)
        {
            if (GalaxyApi.connection.isConnected) GalaxyApi.instances.InstanceList();
        }


        void AddItem(InstanceInfo info)
        {           
                WidgetGroupBox test = new WidgetGroupBox(gui);
                WidgetLabel name = new WidgetLabel(gui, info.name);                
                WidgetLabel players = new WidgetLabel(gui, "Players: "+ info.clients + " / " + info.maxClients);
                WidgetButton button = new WidgetButton(gui, "Connect");
                button.Data = info.id.ToString();
                WidgetSpacer spacer = new WidgetSpacer(gui);
                spacer.Width = 10;
                
                button.AddCallback(Gui.CALLBACK_INDEX.CLICKED, ClickConnect);
                test.AddChild(name);
                test.AddChild(players);
                test.AddChild(button);
                test.AddChild(spacer);
                content.AddChild(test);
                items.Add(test);
        }


        public void Close()
        {
            if (ui == null) return;
            GalaxyEvents.OnGalaxyInstancesList -= OnGalaxyInstancesList;
            ui.DeleteLater();
        }

        private void ClickCreate(Widget widget)
        {
            string name = (ui.GetWidget(ui.FindWidget("room_name")) as WidgetEditLine).Text;
            string count_s = (ui.GetWidget(ui.FindWidget("room_maxPlayers")) as WidgetEditLine).Text;
            int count = 0;
            if (!System.Int32.TryParse(count_s, out count)) count = 1000;

            // Create a new instance
            // Создаем новый инстанс
            GalaxyApi.instances.Create(name, count);
        }

        private void ClickConnect(Widget widget)
        {
            Log.Message(widget.Data);
            int instanceID = 0;
            if (System.Int32.TryParse(widget.Data, out instanceID))
            {
                GalaxyApi.instances.EnterToInstance(instanceID);
            }
            
        }
    }
}

 