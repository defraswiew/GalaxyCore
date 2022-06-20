using GalaxyCoreLib;
using GalaxyNetwork.Core.Scripts;
using SimpleMmoCommon.Messages;
using UnityEngine;
using UnityEngine.UI;
namespace GalaxyCoreLib
{
    /// <summary>
    /// Упрощенный пример окна регистрации
    /// </summary>
    public class GalaxyUIReg : MonoBehaviour
    {
        /// <summary>
        /// активна ли регистрация
        /// </summary>
        public bool active = false;
        /// <summary>
        /// Поле логина
        /// </summary>
        [SerializeField]
        public InputField login;
        /// <summary>
        /// Поле пароля
        /// </summary>
        [SerializeField]
        public InputField password;
        /// <summary>
        /// Поле проверки пароля
        /// </summary>
        [SerializeField]
        public InputField password2;
        /// <summary>
        /// Куда выводим статус
        /// </summary>
        [SerializeField]
        public Text status;
        /// <summary>
        /// анимашка прогресса
        /// </summary>
        [SerializeField]
        public GameObject progress;

        private GalaxyConnection _connection;

        void Start()
        {
            _connection = GalaxyNetworkController.Api.MainConnection;
            if (active) status.text = "";
        }
        /// <summary>
        /// Вызываем регистрацию
        /// </summary>
        public void Reg()
        {
            if (!active)
            {
                status.text = "Регистрация отключена";
                return;
            }
            if (_connection.Api.Transport.IsConnected)
            {
                status.text = "Подключение активно";
                return;
            }
            if (login.text.Length < 4)
            {
                status.text = "Какой то очень короткий логин";
                return;
            }
            if (password.text.Length < 4)
            {
                status.text = "А где пароль?";
                return;
            }
            if (password.text != password2.text)
            {
                status.text = "Пароли не совпадают(";
                return;
            }
            // Создаем новое сообщение для регистрации
            MessageAuth messageAuth = new MessageAuth();
            messageAuth.Login = login.text;
            messageAuth.Password = password.text;
            status.text = "Региструемся";
            // отправляем запрос регистрации на сервер
         
            _connection.Registration(messageAuth.Serialize());
            progress.SetActive(true);
        }

    }
}
