using System.Collections.Generic;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using Unigine;

namespace UnigineApp.data.GalaxyNetwork
{
    [Component(PropertyGuid = "164ae8fdf7c696389cd6e4371ba5742ae8389581")]
    public class GalaxyNetworkInstantiator : Component
    {
    
        private Dictionary<string, AssetLinkNode> _cache;

        private void Init()
        {
            _cache = new Dictionary<string, AssetLinkNode>();
            // Subscribing to the event of creating a new entity
            GalaxyEvents.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
        }

        // Helper method for getting an asset by name
        private AssetLinkNode GetAssetByName(string name)
        {
            if (_cache.TryGetValue(name, out var result))
            {
                return result;
            }
            var files = new List<string>();
            FileSystem.GetVirtualFiles(files);
            var found = files.FindAll(file => System.IO.Path.GetFileName(file) == name + ".node");
            if (found.Count == 0) return null;
            result = new AssetLinkNode(found[0]);
            _cache.Add(name, result);
            return result;
        }

        private ClientNetEntity OnGalaxyNetEntityInstantiate(ClientNetEntity netEntity)
        {
            //Log.Message(netEntity.NetID + " \n");
            // If there is no asset name, then exit
            if (netEntity.PrefabName == "") return null;
            var assetLink = GetAssetByName(netEntity.PrefabName);
            if (!assetLink.IsFileExist)
            {
                Console.WarningLine("not found");
                Log.Error("asset " + netEntity.PrefabName + " not found");
                return null;
            }
       
            var node = assetLink.Load();
            var go = node.AddComponent<UnigineNetEntity>();
            if (go.NetEntity == null) go.NetEntity = netEntity;

            return go.NetEntity;
        }
    }
}