using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;

namespace SimpleMmoCommon.Messages
{
    /// <summary>
    /// Сообщение успешной авторизации
    /// </summary>
    [GalaxySerializable]
    public class MessageApproval:GalaxyMessage
    {
        /// <summary>
        /// Ид клиента
        /// </summary>
        [GalaxyField(0)]
        public string Name;
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
            Name = name;
        }
    }
}
