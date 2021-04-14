using UnityEngine;
using UnityEngine.UI;

namespace GalaxyCoreLib
{
    /// <summary>
    /// Строка комнаты в окне списка
    /// </summary>
    public class GalaxyUiRoomItem : MonoBehaviour
    {
        /// <summary>
        /// Имя комнаты
        /// </summary>
        [SerializeField]
        private Text roomName;
        /// <summary>
        /// Число игроков
        /// </summary>
        [SerializeField]
        private Text userCount;
        /// <summary>
        /// Открытая ли комната
        /// </summary>
        [SerializeField]
        private GameObject open;
        /// <summary>
        /// Закрытыя ли комната
        /// </summary>
        [SerializeField]
        private GameObject close;
        /// <summary>
        /// Изображение комнаты если есть
        /// </summary>
        [SerializeField]
        private Image logo;
        /// <summary>
        /// Пароль на вход в комнату
        /// </summary>
        [SerializeField]
        private GameObject passField;
        /// <summary>
        /// Открыта ли комната
        /// </summary>
        private bool isOpen;
        /// <summary>
        /// Текущий указанный пользователем пароль
        /// </summary>
        private string password = "";
        /// <summary>
        /// Ид комнаты
        /// </summary>
        private int id;

        /// <summary>
        /// Инициализация строки
        /// </summary>
        /// <param name="roomId">ид инстанса</param>
        /// <param name="nameRoom">имя</param>
        /// <param name="count">текущее число пользователей</param>
        /// <param name="max">максимальное число пользователей</param>
        /// <param name="isOpen">открыта ли комната</param>
        /// <param name="img">картиночка если есть</param>
        public void Init(int roomId, string nameRoom, int count, int max, bool isOpen, Sprite img = null)
        {
            id = roomId;
            roomName.text = nameRoom;
            userCount.text = count + " /" + max;
            this.isOpen = isOpen;
            if (isOpen)
            {
                open.SetActive(true);
            }
            else
            {
                close.SetActive(true);
            }
            if (img != null) logo.sprite = img;
        }
        /// <summary>
        /// Обновляем пароль из поля passField
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            this.password = password;
        }
        /// <summary>
        /// Пытаемся подключиться к комнате
        /// </summary>
        public void Connect()
        {
            if (!isOpen && password == "")
            {
                passField.SetActive(true);
                // нельзя подключиться если инстанс с паролем 
                // а пароль не указан
                return;
            }
            // отправляем в апи запрос на подключение к инстансу
            GalaxyApi.Instances.EnterToInstance(id, password);
        }
    }
}
