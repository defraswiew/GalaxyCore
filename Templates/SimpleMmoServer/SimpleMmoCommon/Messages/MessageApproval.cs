using GalaxyCoreCommon;
using ProtoBuf; 

namespace SimpleMmoCommon.Messages
{
    /// <summary>
    /// Сообщение успешной авторизации
    /// </summary>
    [ProtoContract]
    public class MessageApproval:BaseMessage
    {
        /// <summary>
        /// Ид клиента
        /// </summary>
        [ProtoMember(1)]
        public string name; 
        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public MessageApproval()
        {

        }
        /// <summary>
        /// Конструктор с именем
        /// </summary>
        /// <param name="name"></param>
        public MessageApproval(string name)
        {
            this.name = name;
        }
    }
}
