using GalaxyTemplateCommon.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon
{
   public class BaseNetGO
    {
        public string name { get; set; }
        
        public int localId { get; set; }
        
        public int netID { get; set; }
        
        public MessageVector3 position { get; set; }
        
        public MessageQuaternion rotation { get; set; }
       
        public int owner { get; set; }

    }
}
