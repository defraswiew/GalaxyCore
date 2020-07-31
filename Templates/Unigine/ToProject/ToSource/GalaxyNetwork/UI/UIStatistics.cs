using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System;
using System.Collections.Generic;
using System.Text;
using Unigine;

namespace UnigineApp.GalaxyNetwork
{
  public  class UIStatistics
    {
        public UserInterface ui;
        Widget content;
        Gui gui;
        WidgetLabel inTraffic;
        WidgetLabel inSpeed;
        WidgetLabel outTraffic;
        WidgetLabel outSpeed;
        WidgetLabel entity;
        WidgetLabel ping;
        public void Drow()
        {
            gui = Gui.Get();
            if (ui == null) ui = new UserInterface(gui, "GalaxyNetwork/UI/galaxyStatistics.ui");
            ui.AddCallback("close", Gui.CALLBACK_INDEX.CLICKED, ClickClose);
            Widget window = ui.GetWidget(ui.FindWidget("window"));
            window.Arrange();
            gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_RIGHT | Gui.ALIGN_RIGHT);
            content = ui.GetWidget(ui.FindWidget("content"));

            inTraffic = new WidgetLabel(gui, "");
            content.AddChild(inTraffic, Gui.ALIGN_LEFT);
            inSpeed = new WidgetLabel(gui, "");
            content.AddChild(inSpeed, Gui.ALIGN_LEFT);
            outTraffic = new WidgetLabel(gui, "");
            content.AddChild(outTraffic, Gui.ALIGN_LEFT);
            outSpeed = new WidgetLabel(gui, "");
            content.AddChild(outSpeed, Gui.ALIGN_LEFT);
            entity = new WidgetLabel(gui, "");
            content.AddChild(entity, Gui.ALIGN_LEFT);
            ping = new WidgetLabel(gui, "");
            content.AddChild(ping, Gui.ALIGN_LEFT);

            GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
        }

        private void OnFrameUpdate()
        {
            if (!GalaxyApi.connection.isConnected) return;

            inTraffic.Text = "Incoming traffic: " + System.Math.Round((GalaxyApi.connection.statistic.inTraffic / 1024f) / 1024f, 2) + " MB";
            inSpeed.Text = "Incoming speed: " + System.Math.Round(GalaxyApi.connection.statistic.inTrafficInSecond / 1024f, 2) + " KB";           
            outTraffic.Text = "Outgoing traffic: "+ System.Math.Round((GalaxyApi.connection.statistic.outTraffic / 1024f) / 1024f, 2) + " MB";
            outSpeed.Text = "Outgoing speed: " + System.Math.Round(GalaxyApi.connection.statistic.outTrafficInSecond / 1024f, 2) + " KB";
            entity.Text = "NetEntities: " +  GalaxyApi.netEntity.Count.ToString();
            ping.Text = "Ping: " + System.Math.Round(GalaxyApi.connection.statistic.ping * 500, 2) + " ms";
        }

        public void Close()
        {
            if (ui == null) return;
            GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
            ui.DeleteLater();
        }

        private void ClickClose(Widget widget)
        {
            Close();
        }
    }
}