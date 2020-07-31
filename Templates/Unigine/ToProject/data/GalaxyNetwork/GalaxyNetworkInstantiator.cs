using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "164ae8fdf7c696389cd6e4371ba5742ae8389581")]
public class GalaxyNetworkInstantiator : Component
{
    
    private Dictionary<string, AssetLinkNode> cache;

    private void Init()
    {
        cache = new Dictionary<string, AssetLinkNode>();
        // Subscribing to the event of creating a new entity
        // Подписываемся на событие создание новой сущности
        GalaxyEvents.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
    }

    private void Update()
    {

    }
    // Helper method for getting an asset by name
    // Вспомогательный метод получения ассета по имени
    private AssetLinkNode GetAssetByName(string name)
    {
        AssetLinkNode result = null;
        if (cache.TryGetValue(name, out result))
        {
            return result;
        }
        List<string> files = new List<string>();
        FileSystem.GetVirtualFiles(files);
        List<string> found = files.FindAll(file => System.IO.Path.GetFileName(file) == name + ".node");
        if (found.Count == 0) return null;
        result = new AssetLinkNode(found[0]);
        cache.Add(name, result);
        return result;
    }

    private ClientNetEntity OnGalaxyNetEntityInstantiate(ClientNetEntity netEntity)
    {
        Log.Message(netEntity.netID + " \n");
        // If there is no asset name, then exit
        // Если нет имени ассета то выходим
        if (netEntity.prefabName == "") return null;
         AssetLinkNode assetLink = GetAssetByName(netEntity.prefabName);
        if (!assetLink.IsFileExist)
        {
            Log.Error("asset " + netEntity.prefabName + " not found");
            return null;
        }
       
        Node node = assetLink.Load();
        UnigineNetEntity go = node.AddComponent<UnigineNetEntity>();
        if (go.netEntity == null) go.netEntity = netEntity;

        return go.netEntity;
    }
}