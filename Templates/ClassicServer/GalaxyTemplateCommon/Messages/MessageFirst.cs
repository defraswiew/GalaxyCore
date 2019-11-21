using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    /// <summary>
    /// Пример первого пакета с данными
    /// </summary>
    [ProtoContract]
   public class MessageFirst:BaseMessage
    {
        /// <summary>
        /// Id
        /// </summary>
        [ProtoMember(1)]
        public int id { get; set; }
    }
}
