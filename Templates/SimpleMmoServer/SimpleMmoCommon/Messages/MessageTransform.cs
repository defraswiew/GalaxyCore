using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoCommon.Messages
{
    /// <summary>
    /// Пример синхронизации трансформа
    /// </summary>
    [ProtoContract]
   public class MessageTransform:BaseMessage
    {
        /// <summary>
        /// Позиция
        /// </summary>
        [ProtoMember(1)]
        public GalaxyVector3 position { get; set; }
        /// <summary>
        /// Поворот
        /// </summary>
        [ProtoMember(2)]
        public GalaxyQuaternion rotation { get; set; }
    }
}
