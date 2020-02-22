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


        private NetEntity OnNetEntityInstantiate(string name, byte[] data, Client client)
        {
            switch (name)
            {
                case "Pet":
                    ExamplePet pet = new ExamplePet();
                    return pet;   
                default:
                    ExampleNetEntity netEntity = new ExampleNetEntity();
                      return netEntity;
            }
           // return null;           
        }

    }
}
