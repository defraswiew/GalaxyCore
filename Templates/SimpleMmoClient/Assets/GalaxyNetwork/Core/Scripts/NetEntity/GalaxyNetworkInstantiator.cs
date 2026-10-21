using System.Collections.Generic;
using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyNetwork.Core.Scripts.NetEntity
{
    public class GalaxyNetworkInstantiator : MonoBehaviour
    {
        private readonly Dictionary<string, UnityNetEntity> _entityPrefabs = new Dictionary<string, UnityNetEntity>();
        private GalaxyConnection _connection;
        private GameObject _staging;

        private void OnEnable()
        {
            _entityPrefabs.Clear();
            foreach (var prefab in Resources.LoadAll<UnityNetEntity>(""))
            {
                if (_entityPrefabs.ContainsKey(prefab.name))
                    Debug.LogError("Duplicate Galaxy prefab name: " + prefab.name, prefab);
                else _entityPrefabs.Add(prefab.name, prefab);
            }
            _staging = new GameObject("Galaxy incoming staging");
            _staging.SetActive(false);
            // Share the factory's scene/lifetime, including DontDestroyOnLoad controllers.
            _staging.transform.SetParent(transform, true);
            _connection = GalaxyNetworkController.Api.MainConnection;
            _connection.Events.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
        }

        private void OnDisable()
        {
            if (_connection != null)
                _connection.Events.OnGalaxyNetEntityInstantiate -= OnGalaxyNetEntityInstantiate;
            if (_staging != null) Destroy(_staging);
            _connection = null;
            _entityPrefabs.Clear();
        }

        private ClientNetEntity OnGalaxyNetEntityInstantiate(ClientNetEntity incoming)
        {
            if (string.IsNullOrEmpty(incoming.PrefabName)) return null;
            if (!_entityPrefabs.TryGetValue(incoming.PrefabName, out var prefab))
            {
                Debug.LogWarning("Galaxy prefab not found: " + incoming.PrefabName);
                return null;
            }
            // Instantiate inactive so every Awake/OnEnable sees the original incoming entity,
            // including owner, authority, parent, init data and initial GalaxyVars.
            var view = Instantiate(prefab, incoming.transform.Position.Vector3(),
                incoming.transform.Rotation.Quaternion(), _staging.transform);
            view.Bind(incoming);
            view.transform.localScale = incoming.transform.LocalScale.Vector3();
            view.transform.SetParent(null, true);
            // Views belong to the gameplay scene, even when their staging parent is persistent.
            SceneManager.MoveGameObjectToScene(view.gameObject, SceneManager.GetActiveScene());
            view.gameObject.SetActive(true);
            return incoming;
        }
    }
}
