using GalaxyCoreCommon;
using GalaxyCoreServer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleMmoServer.RPGTemplate
{
    [ProtoContract]
    public class MapSaver:BaseMessage
    {
        [ProtoMember(1)]
        public List<MapSaverItem> items;

        public void SaveInstance(Instance instance, string saveName)
        {
            items = new List<MapSaverItem>();
            foreach (var item in instance.entities.list)
            {
                if (!item.isStatic) continue;
                MapSaverItem saverItem = new MapSaverItem();
                saverItem.pos = item.transform.position;
                saverItem.rot = item.transform.rotation;
                saverItem.pref = item.prefabName;
                items.Add(saverItem);
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
                items = BaseMessage.Deserialize<MapSaver>(File.ReadAllBytes("Maps/" + saveName)).items;
                if (items == null) return;
                foreach (var item in items)
                {
                    NetEntityStandart netEntity = new NetEntityStandart(instance,item.pos,item.rot);
                    netEntity.isStatic = true;
                    netEntity.prefabName = item.pref;
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
        public GalaxyVector3 pos;
        [ProtoMember(2)]
        public GalaxyQuaternion rot;
        [ProtoMember(3)]
        public string pref;
    }
}
