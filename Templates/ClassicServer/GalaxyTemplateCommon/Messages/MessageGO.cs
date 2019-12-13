using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    [ProtoContract]
   public class MessageGO:BaseMessage
    {
        /// <summary>
        /// Ид объекта которому предназначена команда
        /// </summary>
        [ProtoMember(1)]
        public int netID { get; set; }
        /// <summary>
        /// пользовательский код команды
        /// </summary>
        [ProtoMember(2)]
        public byte command { get; set; }
        /// <summary>
        /// произвольные данные в формате object
        /// </summary>
        [ProtoMember(3)]
        public byte[] data { get; set; }
    }
}
