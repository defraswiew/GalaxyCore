using GalaxyCoreCommon;
using GalaxyCoreServer;
using SimpleMmoCommon.RPGTemplate; 
using System.Collections.Generic;
 

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Пример падающего дропа
    /// </summary>
    public class Drop : NetEntity
    {
        //список итемов хранящихся в дропе
        DropList dropList = new DropList();
        public Drop(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default, NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation, syncType)
        {
            prefabName = "Drop";
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
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
                        if (dropList.items.Contains(itemID))
                        {
                            dropList.items.Remove(itemID);
                            Log.Info("Пользователь " + clientSender.id, "Взял итем id:" + itemID);
                            // если итемы закончились удаляемся
                            if (dropList.items.Count == 0)
                            {
                                Destory();
                            }
                        }
                        else
                        {
                            Log.Info("Пользователь " + clientSender.id, "Хотел взять итем id:" + itemID + "  но такого уже нет");
                        }
                    }
                    break;
            }
        }

        public override void OnDestroy()
        {

        }

        public override void Start()
        {
            // Генерируем случайные итемы
            dropList.items = new List<int>();
            for (int i = 0; i < GRand.NextInt(1, 10); i++)
            {
                dropList.items.Add(GRand.NextInt(1, 100));
            }
            // удаляем дроп если его не собрали за две минуты
            Invoke("DestroByTime", 120);
        }

        public void DestroByTime()
        {
            Destory();
        }

        public override void Update()
        {

        }
    }
}
