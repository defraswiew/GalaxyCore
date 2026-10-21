using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using Unigine;

namespace UnigineApp.source.GalaxyNetwork.UI
{
  public  class UIStatistics
    {
        private UserInterface _ui;
        private Widget _content;
        private Gui _gui;
        private WidgetLabel _inTraffic;
        private WidgetLabel _inSpeed;
        private WidgetLabel _outTraffic;
        private WidgetLabel _outSpeed;
        private WidgetLabel _entity;
        private WidgetLabel _ping;
        private readonly GalaxyConnection _connection;

        public UIStatistics(GalaxyConnection connection) => _connection = connection;

        public void Show()
        {
            if (_ui != null) return;
            _gui = Gui.Get();
            if (_ui == null) _ui = new UserInterface(_gui, "GalaxyNetwork/UI/galaxyStatistics.ui");
            _ui.AddCallback("close", Gui.CALLBACK_INDEX.CLICKED, ClickClose);
            var window = _ui.GetWidget(_ui.FindWidget("window"));
            window.Arrange();
            _gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_RIGHT | Gui.ALIGN_RIGHT);
            _content = _ui.GetWidget(_ui.FindWidget("content"));

            _inTraffic = new WidgetLabel(_gui, "");
            _content.AddChild(_inTraffic, Gui.ALIGN_LEFT);
            _inSpeed = new WidgetLabel(_gui, "");
            _content.AddChild(_inSpeed, Gui.ALIGN_LEFT);
            _outTraffic = new WidgetLabel(_gui, "");
            _content.AddChild(_outTraffic, Gui.ALIGN_LEFT);
            _outSpeed = new WidgetLabel(_gui, "");
            _content.AddChild(_outSpeed, Gui.ALIGN_LEFT);
            _entity = new WidgetLabel(_gui, "");
            _content.AddChild(_entity, Gui.ALIGN_LEFT);
            _ping = new WidgetLabel(_gui, "");
            _content.AddChild(_ping, Gui.ALIGN_LEFT);

            _connection.Events.OnFrameUpdate += OnFrameUpdate;
        }

        private void OnFrameUpdate()
        {
            if (!_connection.Api.Transport.IsConnected) return;

            _inTraffic.Text = "Incoming traffic: " + System.Math.Round((_connection.Statistic.InTraffic / 1024f) / 1024f, 2) + " MB";
            _inSpeed.Text = "Incoming speed: " + System.Math.Round(_connection.Statistic.InTrafficInSecond / 1024f, 2) + " KB/s";
            _outTraffic.Text = "Outgoing traffic: "+ System.Math.Round((_connection.Statistic.OutTraffic / 1024f) / 1024f, 2) + " MB";
            _outSpeed.Text = "Outgoing speed: " + System.Math.Round(_connection.Statistic.OutTrafficInSecond / 1024f, 2) + " KB/s";
            _entity.Text = "NetEntities: " + _connection.Api.NetEntity.Count;
            _ping.Text = "Ping (RTT): " + System.Math.Round(_connection.Ping * 1000, 2) + " ms";
        }

        public void Close()
        {
            if (_ui == null) return;
            _connection.Events.OnFrameUpdate -= OnFrameUpdate;
            _ui.DeleteLater();
            _ui = null;
        }

        private void ClickClose(Widget widget)
        {
            Close();
        }
    }
}
