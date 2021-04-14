using GalaxyCoreCommon;
using GalaxyCoreLib.Api;
using SimpleMmoCommon.Messages;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace GalaxyCoreLib
{
    /// <summary>
    /// Пример окна авторизации
    /// </summary>
    public class GalaxyUILogin : MonoBehaviour
    {
        [Header("Событие при успешной авторизации")]
        public UnityEvent OnLoginSucesseEvent;
        [Header("Событие ошибки авторизации")]
        public UnityOnLoginError OnLoginErrorEvent;
        /// <summary>
        /// Список объектов которые стоит включить по успешной авторизации
        /// </summary>
        public GameObject[] enableObject;
        /// <summary>
        /// список объектов которые нужно отключить по успешной авторизации
        /// </summary>
        public GameObject[] disableObject;
        /// <summary>
        /// Поле логина
        /// </summary>
        [Space(15)]
        [SerializeField]
        public InputField login;
        /// <summary>
        /// Поле пароля
        /// </summary>
        [SerializeField]
        public InputField password;
        /// <summary>
        /// куда выводим статус
        /// </summary>
        [SerializeField]
        public Text status;
        /// <summary>
        /// Анимашка прогресса
        /// </summary>
        [SerializeField]
        public GameObject progress;
        /// <summary>
        /// число попыток
        /// </summary>
        private int count = 1;

        private void OnEnable()
        {
            //подписка на событие об ошибках авторизации
            GalaxyEvents.OnGalaxyApprovalResponse += OnGalaxyApprovalResponse;
            // событие успешного коннекта      
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            progress.SetActive(false);
        }

        private void OnGalaxyConnect(byte[] message)
        {
            // Отключаем ожидалку
            CancelInvoke("Waiting");
            // если есть подписавшиеся, вызываем
            if (OnLoginSucesseEvent != null) OnLoginSucesseEvent.Invoke();
            // включаем то что нас просили
            for (int i = 0; i < enableObject.Length; i++)
            {
                enableObject[i].SetActive(true);
            }
            // выклачем то что просили
            for (int i = 0; i < disableObject.Length; i++)
            {
                disableObject[i].SetActive(false);
            }
            // А это делать не обязательно, это просто пример чтения ответа на авторизацию
            MessageApproval messageApproval = new MessageApproval();
            messageApproval = BaseMessage.Deserialize<MessageApproval>(message);
            Debug.Log("Наше имя " + messageApproval.Name);
            // прячем окно
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            // отписываемся от всего на что подписались
            GalaxyEvents.OnGalaxyApprovalResponse -= OnGalaxyApprovalResponse;
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        }
        /// <summary>
        /// Сюда приходит аварийный ответ авторизации, значит что то пошло не так
        /// </summary>
        /// <param name="code">код того что пошло не так</param>
        /// <param name="message">текстовое описание</param>
        private void OnGalaxyApprovalResponse(byte code, string message)
        {
            status.text = message;
            progress.SetActive(false);
            if (OnLoginErrorEvent != null) OnLoginErrorEvent.Invoke(code, message);
        }
        /// <summary>
        /// Начинаем авторизацию
        /// </summary>
        public void Auth()
        {
            // выключаем ожидалку если она была включена
            CancelInvoke("Waiting");
            //Создаем новое сообщение аунтефикации, которое мы положили в GalaxyTemplateCommon
            MessageAuth messageAuth = new MessageAuth();
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
            if (count == 1) status.text = "Подключение...";
            messageAuth.Login = login.text;
            messageAuth.Password = password.text;
            // Отправляем сообщение авторизации
            GalaxyApi.Connection.Connect(messageAuth.Serialize());           
            // запускаем отображение прогресса
            progress.SetActive(true);
            // для информации выводим версию клиента
            Debug.Log("Client version: " + GalaxyClientCore.Version.ToString());
            Invoke("Waiting", 5f);
        }
        public void DropCount()
        {
            count = 1;
        }

        /// <summary>
        /// Ожидание подключения
        /// </summary>
        private void Waiting()
        {
            count++;
            if (count > 3)
            {
                status.text = "Подключиться не удалось";
                progress.SetActive(false);
                return;
            }
            status.text = "Попытка № " + count;
            Auth();
        }

    }
}
/// <summary>
/// Эвент ошибки авторизации
/// </summary>
[System.Serializable]
public class UnityOnLoginError : UnityEvent<byte, string>
{

}