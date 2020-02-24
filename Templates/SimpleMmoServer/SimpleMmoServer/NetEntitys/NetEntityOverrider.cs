using GalaxyCoreServer;
using GalaxyCoreServer.Api;
 

namespace SimpleMmoServer
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
                   Examples.NetEntitys.ExamplePet pet = new Examples.NetEntitys.ExamplePet();
                    return pet;
                case "Player":
                    Examples.NetEntitys.ExamplePlayer player = new Examples.NetEntitys.ExamplePlayer();
                    return player;
                default:                    
                      return null;
            }
         
        }

    }
}
