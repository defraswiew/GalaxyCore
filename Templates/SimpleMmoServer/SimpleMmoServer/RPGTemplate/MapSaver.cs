using GalaxyCoreCommon;
using GalaxyCoreServer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO; 

namespace SimpleMmoServer.RPGTemplate
{
    /// <summary>
    /// Очень грубый пример сохранения статических сущностей 
    /// в бинарный файлик, и загрузки обратно.
    /// </summary>
    [ProtoContract]
    public class MapSaver:BaseMessage
    {
        [ProtoMember(1)]
        public List<MapSaverItem> Items;

        public void SaveInstance(Instance instance, string saveName)
        {
            Items = new List<MapSaverItem>();
            foreach (var item in instance.Entities.List)
            {
                if (!item.IsStatic) continue;
                MapSaverItem saverItem = new MapSaverItem
                {
                    Position = item.transform.Position, 
                    Rotation = item.transform.Rotation, 
                    PrefabName = item.PrefabName
                };
                Items.Add(saverItem);
            }
            if (!Directory.Exists("Maps"))
            {
                DirectoryInfo di = Directory.CreateDirectory("Maps");
            }
            File.WriteAllBytes("Maps/" + saveName,this.Serialize());
            Log.Info("Instanse Save", saveName);
        }

        public void Load(Instance instance, string saveName)
        {
            try
            {
                Items = BaseMessage.Deserialize<MapSaver>(File.ReadAllBytes("Maps/" + saveName)).Items;
                if (Items == null) return;
                foreach (var item in Items)
                {
                    var netEntity = new NetEntityStandard(instance, item.Position, item.Rotation)
                    {
                        IsStatic = true, 
                        PrefabName = item.PrefabName
                    };
                    netEntity.Init();
                }
            }
            catch (Exception)
            {
                Log.Info("Instanse Save", "can not read " + saveName);
            }
        }
    }
    
    [ProtoContract]
    public class MapSaverItem : BaseMessage
    {
        [ProtoMember(1)]
        public GalaxyVector3 Position;
        [ProtoMember(2)]
        public GalaxyQuaternion Rotation;
        [ProtoMember(3)]
        public string PrefabName;
    }
}
