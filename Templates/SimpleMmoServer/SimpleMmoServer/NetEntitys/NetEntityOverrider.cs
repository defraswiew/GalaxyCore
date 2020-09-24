using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.Examples.NetEntitys;
using SimpleMmoServer.RPGTemplate;

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
                case "Player":
                    Examples.NetEntitys.ExamplePlayer player = new Examples.NetEntitys.ExamplePlayer(client.instanse);
                    return player;
                case "ExampleChangeOwner":
                    Examples.NetEntitys.ExampleChangeOwner exampleChangeOwner = new Examples.NetEntitys.ExampleChangeOwner(client.instanse);
                    return exampleChangeOwner;
                case "ExampleVideo":
                    ExampleVideo exampleVideo = new ExampleVideo(client.instanse);
                    return exampleVideo;
                case "RPGTemplatePlayer":
                    RPGTemplatePlayer rPGTemplatePlayer = new RPGTemplatePlayer(client.instanse);
                    return rPGTemplatePlayer;
                case "Box":
                    ExampleBox exampleBox = new ExampleBox(client.instanse);
                    return exampleBox;
                    
                     


                default:
                    if (name.Contains("Bld_"))
                    {
                        NetEntityStandart netEntity = new NetEntityStandart(client.instanse);                      
                        netEntity.isStatic = true;
                        netEntity.lossOwner = NetEntityLossOwnerLogic.setServer;
                        return netEntity;
                    }
                      return null;
            }
         
        }

    }
}
