using System.Collections.Generic;
using GalaxyCoreLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>
    /// Main network controller
    /// </summary>
    public class GalaxyNetworkController : MonoBehaviour
    {
        /// <summary>
        /// Server ip address
        /// </summary>
        public string ServerIP = "127.0.0.1";

        /// <summary>
        /// Server port
        /// </summary>
        public int ServerPort = 30200;

        /// <summary>
        /// Client configuration
        /// </summary>
        private readonly Config _config = new Config();

        /// <summary>
        /// whether to show labels over entities
        /// </summary>
        public bool DrawLabels = true;

        /// <summary>
        /// self-reference
        /// </summary>
        public static GalaxyNetworkController Api;

        public GalaxyConnection MainConnection;
        public Dictionary<string, GalaxyConnection> SlaveConnections;

        private void Awake()
        {
            if (Api != null)
            {
                Destroy(gameObject);
                return;
            }

            Api = this;
            SlaveConnections = new Dictionary<string, GalaxyConnection>();
            _config.ServerIp = ServerIP;
            _config.ServerPort = ServerPort;
            _config.AppName = "SimpleMmoServer";
            _config.FrameRate = 25;
            MainConnection = new GalaxyConnection(_config);
            var slaveConnect = new GalaxyConnection(_config);
            slaveConnect.EngineCalls.Awake();
            SlaveConnections.Add("test",slaveConnect);
            MainConnection.EngineCalls.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            MainConnection.EngineCalls.Start();
            foreach (var connection in SlaveConnections)
            {
                connection.Value.EngineCalls.Start();
            }
            SceneManager.activeSceneChanged += SceneChanged;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            MainConnection.EngineCalls.OnSceneLoaded();
            foreach (var connection in SlaveConnections)
            {
                connection.Value.EngineCalls.OnSceneLoaded();
            }
        }

        private void SceneChanged(Scene arg0, Scene arg1)
        {
            MainConnection.EngineCalls.OnSceneChange();
            foreach (var connection in SlaveConnections)
            {
                connection.Value.EngineCalls.OnSceneChange();
            }
        }

        private void Update()
        {
            MainConnection.EngineCalls.Update(Time.deltaTime);
            foreach (var connection in SlaveConnections)
            {
                connection.Value.EngineCalls.Update(Time.deltaTime);
            }
        }

        private void OnApplicationQuit()
        {
            MainConnection.Disconnect();
            foreach (var connection in SlaveConnections)
            {
                connection.Value.Disconnect();
            }
        }

        private void OnDestroy()
        {
            if (GalaxyNetworkController.Api == this)
            {
                MainConnection.Disconnect();
                foreach (var connection in SlaveConnections)
                {
                    connection.Value.Disconnect();
                }
            }
            
        }
    }
}