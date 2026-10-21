using GalaxyCoreLib;
using SimpleMmoCommon.Messages;
using Unigine;

namespace UnigineApp.source.GalaxyNetwork.UI
{
   public class UILogin
    {
        private UserInterface _ui;
        private readonly GalaxyConnection _connection;

        public UILogin(GalaxyConnection connection) => _connection = connection;

        public void Show()
        {
            if (_ui != null) return;
            Gui gui = Gui.Get();
            if(_ui==null) _ui = new UserInterface(gui, "GalaxyNetwork/UI/galaxyLogin.ui");
            _ui.AddCallback("auth", Gui.CALLBACK_INDEX.CLICKED, ClickLogin);
            Widget window = _ui.GetWidget(_ui.FindWidget("window"));
            window.Arrange();
            gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
        }

        public void Close()
        {
            if (_ui == null) return;
            _ui.DeleteLater();
            _ui = null;
        }

        private void ClickLogin(Widget widget)
        {
            string login = (_ui.GetWidget(_ui.FindWidget("login_login")) as WidgetEditLine)?.Text;
            string password = (_ui.GetWidget(_ui.FindWidget("login_password")) as WidgetEditLine)?.Text;
            if (string.IsNullOrWhiteSpace(login) || login.Length < 4 || login.Length > 256 ||
                string.IsNullOrEmpty(password) || password.Length > 1024) return;
            // We form an authorization message. (can be replaced with other data)
            // Формируем сообщение аворизации
            var messageAuth = new MessageAuth
            {
                Login = login,
                Password = password
            };
            // We send a request for connection / authorization
            // Отправляем запрос на подключение / авторизацию
            _connection.Connect(messageAuth);
        }
    }
}
