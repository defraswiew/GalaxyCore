using GalaxyCoreCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoCommon.Messages
{
    /// <summary>
    /// Сообщение успешной авторизации
    /// </summary>
    [ProtoContract]
    public class MessageApproval:BaseMessage
    {
        /// <summary>
        /// Ид клиента
        /// </summary>
        [ProtoMember(1)]
        public string name; 
    }
}
