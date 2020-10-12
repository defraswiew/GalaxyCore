using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.Examples.NetEntitys;
using SimpleMmoServer.RPGTemplate;

namespace SimpleMmoServer
{
    /// <summary>
    /// Переопределение сетевых сущностей созданных с стороны клиента
    /// </summary>
   public class NetEntityOverrider
    {
        public NetEntityOverrider()
        {
            // Подписываемся на событие создания сетевой сущности (со стороны клиента)
            GalaxyEvents.OnNetEntityInstantiate += OnNetEntityInstantiate;  
        }

        /// <summary>
        /// Клиент запросил создание сущности
        /// </summary>
        /// <param name="name">Имя префаба</param>
        /// <param name="data">дополнительные данные</param>
        /// <param name="client">Клиент который хочет создать сущность</param>
        /// <returns></returns>
        private NetEntity OnNetEntityInstantiate(string name, byte[] data, Client client)
        {
            // По имени префаба назначаем сущности исполняемый класс
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
