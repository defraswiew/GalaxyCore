using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;

namespace SimpleMmoCommon.Messages
{
    /// <summary>
    ///  Сообщение авторизации
    /// </summary>
    [GalaxySerializable]
    public class MessageAuth : GalaxyMessage
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [GalaxyField(0, MaxLength = 256)]
        public string Login { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [GalaxyField(1, MaxLength = 1024)]
        public string Password { get; set; }
    }
}
