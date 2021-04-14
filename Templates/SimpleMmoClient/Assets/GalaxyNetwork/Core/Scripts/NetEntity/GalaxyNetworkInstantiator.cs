using System.Collections.Generic;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts.NetEntity
{
    /// <summary>
    /// Компонент отвечающий за создание новый сетевых объектов
    /// </summary>
    public class GalaxyNetworkInstantiator : MonoBehaviour
    {
        private readonly Dictionary<string, UnityNetEntity> _entityPrefabs = new Dictionary<string, UnityNetEntity>();

        private void OnEnable()
        {
            GalaxyEvents.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
            foreach (var item in Resources.LoadAll<UnityNetEntity>(""))
            {
                _entityPrefabs.Add(item.name, item);
            }

            Resources.UnloadUnusedAssets();
        }

        private void OnDisable()
        {
            // отписываемся от события создания новых сущностей
            GalaxyEvents.OnGalaxyNetEntityInstantiate -= OnGalaxyNetEntityInstantiate;
        }

        private ClientNetEntity OnGalaxyNetEntityInstantiate(ClientNetEntity netEntity)
        {
            if (netEntity.PrefabName == "") return null;
            if (!_entityPrefabs.TryGetValue(netEntity.PrefabName, out var prefab))
            {
                Debug.LogWarning("Prefab not found" + netEntity.PrefabName);
                return null;
            }

            var go = Instantiate(prefab, netEntity.transform.Position.Vector3(),
                netEntity.transform.Rotation.Quaternion());
            go.NetEntity.transform.Position = netEntity.transform.Position;
            return go.NetEntity;
        }
    }
}