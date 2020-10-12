using GalaxyCoreCommon;
using ProtoBuf; 

namespace SimpleMmoCommon.Messages
{
    /// <summary>
    ///  Сообщение авторизации
    /// </summary>
    [ProtoContract]
    public class MessageAuth : BaseMessage
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [ProtoMember(1)]
        public string login { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [ProtoMember(2)]
        public string password { get; set; }
    }
}
