using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessage : MonoBehaviour
{
    int i;
    void Start()
    {
        GalaxyEvents.OnGalaxyIncommingMessage += GalaxyEvents_OnGalaxyIncommingMessage;
    }

    private void GalaxyEvents_OnGalaxyIncommingMessage(byte code, byte[] data)
    {
        Debug.Log("MessID: " + i + " code: " + code + "  time: " + (int)Time.time);
            i++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
