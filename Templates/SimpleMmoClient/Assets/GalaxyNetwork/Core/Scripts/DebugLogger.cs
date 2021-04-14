using GalaxyCoreLib.Api;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>
    /// An example of a logger that intercepts kernel messages
    /// </summary>
    public class DebugLogger : MonoBehaviour
    {
        private void OnEnable()
        {
            GalaxyEvents.OnGalaxyLogInfo += OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarning += OnGalaxyLogWarning;
            GalaxyEvents.OnGalaxyLogError += OnGalaxyLogError;        
        }

        private void OnDisable()
        {
            GalaxyEvents.OnGalaxyLogInfo -= OnGalaxyLogInfo;
            GalaxyEvents.OnGalaxyLogWarning -= OnGalaxyLogWarning;
            GalaxyEvents.OnGalaxyLogError -= OnGalaxyLogError;
        }

        private void OnGalaxyLogError(string publisher, string message)
        {
            Debug.LogError(publisher + "-> " + message);
        }

        private void OnGalaxyLogWarning(string publisher, string message)
        {
            Debug.LogWarning(publisher + "-> " + message);
        }

        private void OnGalaxyLogInfo(string publisher, string message)
        {
            Debug.Log(publisher + "-> " + message);
        }
    }
}