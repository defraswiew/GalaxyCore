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

        private void Awake()
        {
            if (Api != null)
            {
                Destroy(gameObject);
                return;
            }

            Api = this;
            _config.ServerIp = ServerIP;
            _config.ServerPort = ServerPort;
            _config.AppName = "SimpleMmoServer";
            _config.FrameRate = 25;
            GalaxyClientCore.Initialize(_config);
            GalaxyClientCore.EngineCalls.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            GalaxyClientCore.EngineCalls.Start();
            SceneManager.activeSceneChanged += SceneChanged;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GalaxyClientCore.EngineCalls.OnSceneLoaded();
        }

        private void SceneChanged(Scene arg0, Scene arg1)
        {
            GalaxyClientCore.EngineCalls.OnSceneChange();
        }

        private void Update()
        {
            GalaxyClientCore.EngineCalls.Update(Time.deltaTime);
        }

        private void OnApplicationQuit()
        {
            GalaxyApi.Connection.Disconnect();
        }
    }
}