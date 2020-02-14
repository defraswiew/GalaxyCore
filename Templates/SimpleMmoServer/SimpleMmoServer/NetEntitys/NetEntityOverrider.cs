using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.NetEntitys
{
   public class NetEntityOverrider
    {
        public NetEntityOverrider()
        {
            GalaxyEvents.OnNetEntityInstantiate += OnNetEntityInstantiate;  
        }


        private NetEntity OnNetEntityInstantiate(string name, byte[] data, ClientConnection clientConnection)
        {
            ExampleNetEntity netEntity = new ExampleNetEntity();
            return netEntity;
        }

    }
}
