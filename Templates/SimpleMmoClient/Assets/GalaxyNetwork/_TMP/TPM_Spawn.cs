using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using GalaxyCoreLib;
using GalaxyNetwork.Core.Scripts;
using UnityEngine;

public class TPM_Spawn : MonoBehaviour
{
    public GameObject test;
    public int max = 10;
    private int count;
    private GalaxyConnection _connection;
    void Start()
    {
        _connection = GalaxyNetworkController.Api.MainConnection;
        _connection.Events.OnGalaxyEnterInInstance += GalaxyEvents_OnGalaxyEnterInInstance;
    
    }

    private void GalaxyEvents_OnGalaxyEnterInInstance(GalaxyCoreCommon.InstanceInfo info)
    {
        InvokeRepeating("Spawn", 5, 5);
    }

    void Spawn()
    {
        if (count > max) return;
        GameObject go = Instantiate(test,transform);
        go.transform.position = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        count++;
    }
    
}
