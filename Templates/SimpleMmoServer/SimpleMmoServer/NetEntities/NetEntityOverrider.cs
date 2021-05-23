using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoServer.Examples.NetEntities;
using SimpleMmoServer.RPGTemplate;

namespace SimpleMmoServer.NetEntities
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
        private NetEntity OnNetEntityInstantiate(string name, byte[] data, BaseClient client)
        {
            // По имени префаба назначаем сущности исполняемый класс
            switch (name)
            {
                case "ExampleChangeOwner":
                    Examples.NetEntities.ExampleChangeOwner exampleChangeOwner =
                        new Examples.NetEntities.ExampleChangeOwner(client.Instanse);
                    return exampleChangeOwner;
                case "ExampleVideo":
                    ExampleVideo exampleVideo = new ExampleVideo(client.Instanse);
                    return exampleVideo;
                case "RPGTemplatePlayer":
                    RPGTemplatePlayer rPGTemplatePlayer = new RPGTemplatePlayer(client.Instanse);
                    return rPGTemplatePlayer;

                default:
                    if (name.Contains("Bld_"))
                    {
                        var netEntity = new NetEntityStandard(client.Instanse)
                        {
                            IsStatic = true,
                            LossOwner = NetEntityLossOwnerLogic.setServer
                        };

                        return netEntity;
                    }

                    return null;
            }
        }
    }
}