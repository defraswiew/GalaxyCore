using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    [ProtoContract]
   public class MessageTransform:BaseMessage
    {
        [ProtoMember(1)]
        public int netID { get; set; }
        [ProtoMember(2)]
        public MessageVector3 position { get; set; }

        [ProtoMember(3)]
        public MessageQuaternion rotation { get; set; }
    }
}
