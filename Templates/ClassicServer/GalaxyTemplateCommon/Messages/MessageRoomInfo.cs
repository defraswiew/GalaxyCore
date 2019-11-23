using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon.Messages
{
    [ProtoContract]
    public class MessageRoomInfo: BaseMessage
    {
        [ProtoMember(1)]
        public List<RoomInfo> rooms { get; set; }
    }

    [ProtoContract]
    public class RoomInfo:BaseMessage
    {
        [ProtoMember(1)]
        public string name { get; set; }
        [ProtoMember(2)]
        public uint clients { get; set; }
        [ProtoMember(3)]
        public uint maxClients { get; set; }
        [ProtoMember(4)]
        public int id { get; set; }
    }

}
