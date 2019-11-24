using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    [ProtoContract]
    public class MessageInstantiate:BaseMessage
    {
        [ProtoMember(1)]
        public string name { get; set; }
        [ProtoMember(2)]
        public int localId { get; set; }
        [ProtoMember(3)]
        public int netID { get; set; }
        [ProtoMember(4)]
        public MessageVector3 position { get; set; }
        [ProtoMember(5)]
        public MessageQuaternion rotation { get; set; }
        [ProtoMember(6)]
        public int owner { get; set; }
    }
}
