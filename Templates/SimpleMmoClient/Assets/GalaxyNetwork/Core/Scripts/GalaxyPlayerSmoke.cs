using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyNetwork.Core.Scripts.NetEntity;
using SimpleMmoCommon.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>Opt-in player integration check. Normal launches create no extra objects.</summary>
    public sealed class GalaxyPlayerSmoke : MonoBehaviour
    {
        private string _endpoint;
        private GalaxyConnection _connection;
        private string _failure;
        private string _approvedName;
        private List<InstanceInfo> _rooms;
        private bool _finished;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            string argument = Environment.GetCommandLineArgs().FirstOrDefault(value => value.StartsWith("--galaxy-smoke=", StringComparison.Ordinal));
            if (argument == null) return;
            var go = new GameObject("Galaxy player smoke");
            DontDestroyOnLoad(go);
            var check = go.AddComponent<GalaxyPlayerSmoke>();
            check._endpoint = argument.Substring("--galaxy-smoke=".Length);
            check.StartCoroutine(check.Run());
        }

        private IEnumerator Run()
        {
            Application.logMessageReceived += OnLog;
            float deadline = Time.realtimeSinceStartup + 30f;
            while (GalaxyNetworkController.Api == null && Time.realtimeSinceStartup < deadline) yield return null;
            if (GalaxyNetworkController.Api == null) { Finish(false, "Network controller missing"); yield break; }
            // Let normal UI bind its callbacks before using the same controller and factory.
            yield return null;
            yield return null;
            _connection = GalaxyNetworkController.Api.MainConnection;
            int split = _endpoint.LastIndexOf(':');
            if (split < 1 || !int.TryParse(_endpoint.Substring(split + 1), out int port))
            { Finish(false, "Expected --galaxy-smoke=host:port"); yield break; }
            _connection.Config.ServerIp = _endpoint.Substring(0, split);
            _connection.Config.ServerPort = port;
            _connection.Events.OnGalaxyConnect += OnConnected;
            _connection.Events.OnGalaxyInstancesList += OnRooms;
            string login = "test-unity-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            _connection.Connect(new MessageAuth { Login = login, Password = "test-smoke" }.Serialize());
            while (_approvedName == null && _failure == null && Time.realtimeSinceStartup < deadline) yield return null;
            if (_approvedName != login) { Finish(false, _failure ?? "Authentication response timed out or decoded incorrectly"); yield break; }
            if (Environment.GetCommandLineArgs().Contains("--galaxy-smoke-add-force"))
            {
                yield return RunAddForce(login);
                yield break;
            }
            // A fresh Galaxy server deliberately sends no list response while no rooms exist.
            // Create a room first, then verify that the actual list contains the entered room.
            _connection.Api.Instances.Create("unity-smoke-" + login, 2, 2);
            while (_connection.Api.Instances.Current == null && _failure == null && Time.realtimeSinceStartup < deadline) yield return null;
            if (_connection.Api.Instances.Current == null) { Finish(false, _failure ?? "Instance entry timed out"); yield break; }
            _rooms = null;
            _connection.Api.Instances.InstanceList();
            while ((_rooms == null || !_rooms.Any(room => room.Id == _connection.Api.Instances.Current.Id)) &&
                _failure == null && Time.realtimeSinceStartup < deadline) yield return null;
            if (_rooms == null || !_rooms.Any(room => room.Id == _connection.Api.Instances.Current.Id))
            { Finish(false, _failure ?? "Created room missing from instance list"); yield break; }
            _connection.Api.Instances.SyncInstance();
            // This checks the real view, cached initial GalaxyVars, owner identity and subsequent frames.
            ExampleNetVars2 found = null;
            float firstServerTime = 0;
            while (_failure == null && Time.realtimeSinceStartup < deadline)
            {
                var views = FindObjectsByType<UnityNetEntity>(FindObjectsSortMode.None);
                foreach (var view in views)
                {
                    var vars = view.GetComponent<ExampleNetVars2>();
                    if (view.NetEntity == null || !view.NetEntity.IsInit || vars == null || string.IsNullOrEmpty(vars.Name) || vars.Hp != 100) continue;
                    if (!ReferenceEquals(view.NetEntity, _connection.Api.NetEntity.GetById(view.NetEntity.NetID)))
                    { Finish(false, "View factory replaced the incoming SDK entity"); yield break; }
                    if (view.NetEntity.OwnerClientId != _connection.Api.MyId)
                    { Finish(false, "Incoming owner identity was lost"); yield break; }
                    if (found != vars) { found = vars; firstServerTime = vars.ServerTime; }
                    if (vars.ServerTime > firstServerTime)
                    { Finish(true, "Auth, room list/create/entry, world sync, prefab binding, initial GalaxyVars and server updates passed"); yield break; }
                }
                yield return null;
            }
            DescribeWorld();
            Finish(false, _failure ?? "No bound ExampleVideo with initial and changing GalaxyVars before timeout");
        }

        private IEnumerator RunAddForce(string login)
        {
            // Exercise the normal room UI's scene transitions twice on one connection.
            int previousRoom = 0;
            var previousSphereIds = new HashSet<int>();
            for (int attempt = 0; attempt < 2; attempt++)
            {
                float deadline = Time.realtimeSinceStartup + 30f;
                _connection.Api.Instances.Create("add-force-" + login + "-" + attempt, 2, 1);
                while ((_connection.Api.Instances.Current == null || SceneManager.GetActiveScene().name != "ExamplePhys") &&
                    _failure == null && Time.realtimeSinceStartup < deadline) yield return null;
                if (_failure != null || _connection.Api.Instances.Current == null || SceneManager.GetActiveScene().name != "ExamplePhys")
                { Finish(false, _failure ?? "Add Force scene entry timed out"); yield break; }
                if (_connection.Api.Instances.Current.Id == previousRoom || previousSphereIds.Any(id => _connection.Api.NetEntity.GetById(id) != null))
                { Finish(false, "Add Force re-entry retained the previous room or its SDK entities"); yield break; }
                previousRoom = _connection.Api.Instances.Current.Id;
                previousSphereIds.Clear();

                UnityNetEntity first = null;
                Vector3 startPosition = default;
                bool moved = false;
                int spheres = 0;
                while (_failure == null && Time.realtimeSinceStartup < deadline)
                {
                    spheres = 0;
                    foreach (var view in FindObjectsByType<UnityNetEntity>(FindObjectsSortMode.None))
                    {
                        if (view.NetEntity == null || !view.NetEntity.IsInit || view.NetEntity.PrefabName != "ExampleSphere") continue;
                        if (!ReferenceEquals(view.NetEntity, _connection.Api.NetEntity.GetById(view.NetEntity.NetID)))
                        { Finish(false, "Add Force view replaced the incoming SDK entity"); yield break; }
                        if (view.gameObject.scene != SceneManager.GetActiveScene())
                        { Finish(false, "Add Force view escaped the gameplay scene"); yield break; }
                        previousSphereIds.Add(view.NetEntity.NetID);
                        spheres++;
                        if (first == null) { first = view; startPosition = view.transform.position; }
                    }
                    if (first != null && (first.transform.position - startPosition).sqrMagnitude > 0.001f) moved = true;
                    if (spheres >= 3 && moved) break;
                    yield return null;
                }
                if (_failure != null || spheres < 3 || !moved)
                {
                    DescribeWorld();
                    Finish(false, _failure ?? "Add Force did not spawn three bound spheres with motion updates");
                    yield break;
                }
                Debug.Log("GALAXY_ADD_FORCE_PASS: attempt=" + (attempt + 1) + ", spheres=" + spheres + ", moving=" + moved);
                _connection.Api.Instances.ExitInstance();
                while ((_connection.Api.Instances.Current != null || SceneManager.GetActiveScene().name != "Main") &&
                    _failure == null && Time.realtimeSinceStartup < deadline) yield return null;
                if (_failure != null || _connection.Api.Instances.Current != null || SceneManager.GetActiveScene().name != "Main")
                { Finish(false, _failure ?? "Return from Add Force to Main timed out"); yield break; }
                // Finish scene activation and destruction of duplicate scene controllers before re-entry.
                yield return null;
                yield return null;
                if (FindObjectsByType<UnityNetEntity>(FindObjectsSortMode.None)
                    .Any(view => view.NetEntity != null && view.NetEntity.PrefabName == "ExampleSphere"))
                { Finish(false, "Add Force views survived returning to Main"); yield break; }
            }
            Finish(true, "Add Force scene entry, bound moving spheres, exit to Main and repeated entry passed");
        }

        private void DescribeWorld()
        {
            var views = FindObjectsByType<UnityNetEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Debug.Log("GALAXY_SMOKE_WORLD: sdkEntities=" + _connection.Api.NetEntity.Count +
                ", views=" + views.Length + ", clientId=" + _connection.Api.MyId +
                ", instance=" + _connection.Api.Instances.Current?.Id);
            foreach (var entity in _connection.Api.NetEntity.GetAll())
                Debug.Log("GALAXY_SMOKE_SDK: id=" + entity.NetID + ", prefab=" + entity.PrefabName + ", owner=" + entity.OwnerClientId);
            foreach (var view in views)
            {
                var vars = view.GetComponent<ExampleNetVars2>();
                Debug.Log("GALAXY_SMOKE_VIEW: name=" + view.name + ", active=" + view.gameObject.activeInHierarchy +
                    ", id=" + view.NetEntity?.NetID + ", prefab=" + view.NetEntity?.PrefabName +
                    ", owner=" + view.NetEntity?.OwnerClientId + ", vars=" + (vars != null) +
                    ", Name=" + vars?.Name + ", Hp=" + vars?.Hp + ", ServerTime=" + vars?.ServerTime);
            }
        }

        private void OnConnected(byte[] payload)
        {
            if (GalaxyMessage.TryDeserialize<MessageApproval>(payload, out var approval)) _approvedName = approval.Name;
            else _failure = "Malformed approval payload";
        }

        private void OnRooms(List<InstanceInfo> rooms) => _rooms = rooms ?? new List<InstanceInfo>();
        private void OnLog(string message, string trace, LogType type)
        {
            if (!_finished && (type == LogType.Error || type == LogType.Exception || type == LogType.Assert))
                _failure = message;
        }

        private void Finish(bool success, string message)
        {
            _finished = true;
            Application.logMessageReceived -= OnLog;
            Debug.Log((success ? "GALAXY_PLAYER_SMOKE_OK: " : "GALAXY_PLAYER_SMOKE_FAILED: ") + message);
            if (_connection != null)
            {
                _connection.Events.OnGalaxyConnect -= OnConnected;
                _connection.Events.OnGalaxyInstancesList -= OnRooms;
                _connection.Disconnect();
            }
            Application.Quit(success ? 0 : 1);
        }

        private void OnDestroy() => Application.logMessageReceived -= OnLog;
    }
}
