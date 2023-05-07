using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreLib;
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
        private GalaxyConnection _connection;

        private void OnEnable()
        {
            _connection = GalaxyNetworkController.Api.MainConnection;
            _connection.Events.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
            _connection.Events.OnGalaxyEnterInInstance += EventsOnOnGalaxyEnterInInstance;
            foreach (var item in Resources.LoadAll<UnityNetEntity>(""))
            {
                _entityPrefabs.Add(item.name, item);
            }

            Resources.UnloadUnusedAssets();
        }

        private void EventsOnOnGalaxyEnterInInstance(InstanceInfo info)
        {
            Debug.Log("Instance " + info.Name);
        }

        private void OnDisable()
        {
            // отписываемся от события создания новых сущностей
            _connection.Events.OnGalaxyNetEntityInstantiate -= OnGalaxyNetEntityInstantiate;
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