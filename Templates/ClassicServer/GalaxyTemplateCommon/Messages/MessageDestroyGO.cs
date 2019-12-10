using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    /// <summary>
    /// Сообщение об удалении сетевого объекта или массива объектов
    /// </summary>
    [ProtoContract]
    public class MessageDestroyGO:BaseMessage
    {
        /// <summary>
        /// Ид объекта подлежащего удалению
        /// </summary>
        [ProtoMember(1)]
        public int netID { get; set; }
    }
}
