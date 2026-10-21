using System;
using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalaxyNetwork.Core.Scripts.NetEntity
{
    [DefaultExecutionOrder(-900)]
    public class UnityNetEntity : MonoBehaviour
    {
        public ClientNetEntity NetEntity;
        [FormerlySerializedAs("data")]
        [HideInInspector] public byte[] Data = null;
        [FormerlySerializedAs("initTime")]
        [HideInInspector] public float InitTime;
        private bool _registered;
        private bool _networkDestroyed;

        private void Awake()
        {
            if (NetEntity == null)
                Bind(new ClientNetEntity(GalaxyNetworkController.Api.MainConnection));
        }

        /// <summary>Bind the original incoming entity before activating the prefab.</summary>
        public void Bind(ClientNetEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (ReferenceEquals(NetEntity, entity)) return;
            if (NetEntity != null)
            {
                NetEntity.OnNetStart -= OnNetStart;
                NetEntity.OnNetDestroy -= OnNetDestroy;
            }
            NetEntity = entity;
            _registered = false;
            NetEntity.OnNetStart += OnNetStart;
            NetEntity.OnNetDestroy += OnNetDestroy;
            Data = entity.InitData ?? Data;
        }

        private void Start()
        {
            RegisterVariables();
            if (NetEntity.IsInit) return;
            NetEntity.PrefabName = gameObject.name.Replace("(Clone)", "").Trim();
            NetEntity.InitData = Data;
            NetEntity.transform.Position = transform.position.NetworkVector3();
            NetEntity.transform.Rotation = transform.rotation.NetworkQuaternion();
            NetEntity.transform.LocalScale = transform.localScale.NetworkVector3();
            NetEntity.Connection.Api.NetEntity.Instantiate(NetEntity);
        }

        private void RegisterVariables()
        {
            if (_registered || NetEntity == null) return;
            foreach (var component in GetComponents<MonoBehaviour>())
                if (component != null) NetEntity.GalaxyVars.RegistrationClass(component);
            _registered = true;
        }

        private void OnNetStart()
        {
            InitTime = Time.time;
            Data = NetEntity.InitData;
            // Incoming initial vars must be applied before gameplay Start callbacks.
            RegisterVariables();
        }

        private void OnNetDestroy(NetEntityDestroyReason reason, byte[] customData)
        {
            _networkDestroyed = true;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (NetEntity == null) return;
            NetEntity.OnNetStart -= OnNetStart;
            NetEntity.OnNetDestroy -= OnNetDestroy;
            if (!_networkDestroyed && NetEntity.IsInit && NetEntity.IsMy) NetEntity.Destroy();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (GalaxyNetworkController.Api != null && GalaxyNetworkController.Api.DrawLabels && NetEntity != null)
                UnityEditor.Handles.Label(transform.position + Vector3.up, "NetEntity: " + NetEntity.NetID);
        }
#endif
    }
}
