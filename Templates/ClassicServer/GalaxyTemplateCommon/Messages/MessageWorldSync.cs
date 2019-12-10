using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    /// <summary>
    /// Сообщение синхронизации мира
    /// </summary>
    [ProtoContract]
   public class MessageWorldSync:BaseMessage
    {
        [ProtoMember(1)]
        public List<MessageInstantiate> netObjects { get; set; }
    }
}
