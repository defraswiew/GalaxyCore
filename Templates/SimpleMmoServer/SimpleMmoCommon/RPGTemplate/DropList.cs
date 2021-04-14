using GalaxyCoreCommon;
using ProtoBuf;
using System.Collections.Generic;

namespace SimpleMmoCommon.RPGTemplate
{
    /// <summary>
    /// Пример отправки списка ид итемов
    /// </summary>
    [ProtoContract]
    public class DropList:BaseMessage
    {
        /// <summary>
        /// список ид итемов
        /// </summary>
        [ProtoMember(1)]
        public List<int> Items;
    }
}
