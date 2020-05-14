using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_send_to_instance : MonoBehaviour
{
 
    public void Send()
    {
        //test
        GalaxyApi.send.SendMessageToInstance(5, new byte[0]);
    }
}
