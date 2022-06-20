using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using Unigine;

namespace UnigineApp.source.GalaxyNetwork.UI
{
   public class UIInstances
    {
        private UserInterface _ui;
        private Widget _content;
        private Gui _gui;
        private List<Widget> _items;
        public void Show()
        {
            _gui = Gui.Get();
            if (_ui == null) _ui = new UserInterface(_gui, "GalaxyNetwork/UI/galaxyInstances.ui"); 
            Widget window = _ui.GetWidget(_ui.FindWidget("window"));
            window.Arrange();
            _gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
            _ui.AddCallback("update", Gui.CALLBACK_INDEX.CLICKED, ClickUpdate);
            _ui.AddCallback("create", Gui.CALLBACK_INDEX.CLICKED, ClickCreate);
            _content = _ui.GetWidget(_ui.FindWidget("content"));
            _items = new List<Widget>();

            // Instance List Update Event
            // Пришел ответ на запрос списка инстансов
            GalaxyEvents.OnGalaxyInstancesList += OnGalaxyInstancesList;
            // Requesting an up - to - date list of instances
            // Запрашиваем список инстансов
            if (GalaxyApi.Connection.IsConnected) GalaxyApi.Instances.InstanceList();
        }

        private void OnGalaxyInstancesList(List<InstanceInfo> instances)
        {             
            foreach (var item in _items)
            {
                _content.RemoveChild(item);
            }
            _items.Clear();
            foreach (var item in instances)
            {
                AddItem(item);
            }
        }


        void ClickUpdate(Widget widget)
        {
            if (GalaxyApi.Connection.IsConnected) GalaxyApi.Instances.InstanceList();
        }


        void AddItem(InstanceInfo info)
        {           
                WidgetGroupBox test = new WidgetGroupBox(_gui);
                WidgetLabel name = new WidgetLabel(_gui, info.Name);                
                WidgetLabel players = new WidgetLabel(_gui, "Players: "+ info.Clients + " / " + info.MaxClients);
                WidgetButton button = new WidgetButton(_gui, "Connect");
                button.Data = info.Id.ToString();
                WidgetSpacer spacer = new WidgetSpacer(_gui);
                spacer.Width = 10;
                
                button.AddCallback(Gui.CALLBACK_INDEX.CLICKED, ClickConnect);
                test.AddChild(name);
                test.AddChild(players);
                test.AddChild(button);
                test.AddChild(spacer);
                _content.AddChild(test);
                _items.Add(test);
        }


        public void Close()
        {
            if (_ui == null) return;
            GalaxyEvents.OnGalaxyInstancesList -= OnGalaxyInstancesList;
            _ui.DeleteLater();
        }

        private void ClickCreate(Widget widget)
        {
            string name = (_ui.GetWidget(_ui.FindWidget("room_name")) as WidgetEditLine)?.Text;
            string count_s = (_ui.GetWidget(_ui.FindWidget("room_maxPlayers")) as WidgetEditLine)?.Text;
            if (!System.Int32.TryParse(count_s, out var count)) count = 1000;

            // Create a new instance
            // Создаем новый инстанс
            GalaxyApi.Instances.Create(name, count, 2);
        }

        private void ClickConnect(Widget widget)
        {
            Log.Message(widget.Data);
            if (System.Int32.TryParse(widget.Data, out var instanceID))
            {
                GalaxyApi.Instances.EnterToInstance(instanceID);
            }
            
        }
    }
}

 