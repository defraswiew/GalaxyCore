using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    [ProtoContract]
    public class MessageRoomEnter : BaseMessage
    {
        /// <summary>
        /// Имя комнаты
        /// </summary>
        [ProtoMember(1)]
        public string name { get; set; }
        /// <summary>
        /// Ид комнаты
        /// </summary>
        [ProtoMember(2)]
        public int id { get; set; }
    }
}
