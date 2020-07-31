using GalaxyCoreLib;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Unigine;

namespace UnigineApp.GalaxyNetwork
{
   public class UILogin
    {
        public  UserInterface ui;
        public void Drow()
        {           
            Gui gui = Gui.Get();
            if(ui==null) ui = new UserInterface(gui, "GalaxyNetwork/UI/galaxyLogin.ui");
            ui.AddCallback("auth", Gui.CALLBACK_INDEX.CLICKED, ClickLogin);
            Widget window = ui.GetWidget(ui.FindWidget("window"));
            window.Arrange();
            gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);         
        }

        public void Close()
        {
            if (ui == null) return;
            ui.DeleteLater();
        }

        private void ClickLogin(Widget widget)
        {
            string login = (ui.GetWidget(ui.FindWidget("login_login")) as WidgetEditLine).Text;
            string password = (ui.GetWidget(ui.FindWidget("login_password")) as WidgetEditLine).Text;
            if (login.Length < 4) return;
            // We form an authorization message. (can be replaced with other data)
            // Формируем сообщение аворизации
            MessageAuth messageAuth = new MessageAuth();
            messageAuth.login = login;
            messageAuth.password = password;
            // We send a request for connection / authorization
            // Отправляем запрос на подключение / авторизацию
            GalaxyApi.connection.Connect(messageAuth);
        }
    }
}
