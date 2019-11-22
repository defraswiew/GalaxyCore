using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
public class DebugLoger : MonoBehaviour
{
    private void OnEnable()
    {
        GalaxyEvents.OnGalaxyLogInfo += OnGalaxyLogInfo;
        GalaxyEvents.OnGalaxyLogWarnig += OnGalaxyLogWarnig;
        GalaxyEvents.OnGalaxyLogError += OnGalaxyLogError;
    }

    private void OnDisable()
    {
        GalaxyEvents.OnGalaxyLogInfo -= OnGalaxyLogInfo;
        GalaxyEvents.OnGalaxyLogWarnig -= OnGalaxyLogWarnig;
        GalaxyEvents.OnGalaxyLogError -= OnGalaxyLogError;
    }

    private void OnGalaxyLogError(string publisher, string message)
    {
        Debug.LogError(publisher + "-> " + message);
    }

    private void OnGalaxyLogWarnig(string publisher, string message)
    {
        Debug.LogWarning(publisher + "-> " + message);
    }

    private void OnGalaxyLogInfo(string publisher, string message)
    {
        Debug.Log(publisher + "-> " + message);
    }
}
