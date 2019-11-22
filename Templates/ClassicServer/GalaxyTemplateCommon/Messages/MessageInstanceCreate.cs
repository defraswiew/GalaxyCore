using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    [ProtoContract]
    public class MessageInstanceCreate : BaseMessage
    {
        /// <summary>
        /// Имя комнаты
        /// </summary>
        [ProtoMember(1)]
        public string name { get; set; }
        /// <summary>
        /// Максимальное число игроков
        /// </summary>
        [ProtoMember(2)]
        public uint maxClients { get; set; }
    }
}
