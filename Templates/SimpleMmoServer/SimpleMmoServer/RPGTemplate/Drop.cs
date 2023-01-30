using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate; 
using System.Collections.Generic;
#if GALAXY_DOUBLE
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример падающего дропа
    /// </summary>
    public class Drop : NetEntity
    {
        //список итемов хранящихся в дропе
        DropList dropList = new DropList();
        public Drop(Instance instance, vector position = default, GalaxyQuaternion rotation = default) : base(instance, position, rotation)
        {
            PrefabName = "Drop";
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
            switch (externalCode)
            {
                // нас спросили о нашем содержимом
                case 1:
                    SendMessageByOctoVisible(externalCode, dropList.Serialize(), GalaxyDeliveryType.reliable);
                    break;
                // кто то пытается что то взять
                case 2:
                    {
                        BitGalaxy message = new BitGalaxy(data);
                        int itemID = message.ReadInt();
                        // проверяем есть ли у нас такой итем
                        if (dropList.Items.Contains(itemID))
                        {
                            dropList.Items.Remove(itemID);
                            Log.Info("Пользователь " + clientSender.Id, "Взял итем id:" + itemID);
                            // если итемы закончились удаляемся
                            if (dropList.Items.Count == 0)
                            {
                                Destory();
                            }
                        }
                        else
                        {
                            Log.Info("Пользователь " + clientSender.Id, "Хотел взять итем id:" + itemID + "  но такого уже нет");
                        }
                    }
                    break;
            }
        }

        protected override void OnDestroy()
        {

        }

        protected override void OnRemotePosition(GalaxyVector3 remotePosition)
        {
             
        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {
            
        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {
            
        }

        protected override void Start()
        {
            // Генерируем случайные итемы
            dropList.Items = new List<int>();
            for (int i = 0; i < GRand.NextInt(1, 10); i++)
            {
                dropList.Items.Add(GRand.NextInt(1, 100));
            }
            // удаляем дроп если его не собрали за две минуты
            Invoke("DestroyByTime", 120);
        }

        public void DestroyByTime()
        {
            Destory();
        }

        protected override void Update()
        {

        }
    }
}
