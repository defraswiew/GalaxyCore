using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
public class DebugLoger : MonoBehaviour
{
    private void OnEnable()
    {
        Events.OnGalaxyLogInfo += OnGalaxyLogInfo;
        Events.OnGalaxyLogWarnig += OnGalaxyLogWarnig;
        Events.OnGalaxyLogError += OnGalaxyLogError;
    }

    private void OnDisable()
    {
        Events.OnGalaxyLogInfo -= OnGalaxyLogInfo;
        Events.OnGalaxyLogWarnig -= OnGalaxyLogWarnig;
        Events.OnGalaxyLogError -= OnGalaxyLogError;
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
