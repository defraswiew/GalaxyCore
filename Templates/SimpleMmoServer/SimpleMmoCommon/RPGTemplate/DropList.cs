using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoCommon.RPGTemplate
{
    [ProtoContract]
    public class DropList:BaseMessage
    {
        [ProtoMember(1)]
        public List<int> items;
    }
}
