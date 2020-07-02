using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
public class TestEntity : MonoBehaviour
{
    ClientNetEntity netEntity;
    public void TEST()
    {
        netEntity = new ClientNetEntity();
        netEntity.OnNetStart += OnNetStart;
        netEntity.prefabName = "Player";

         
        GalaxyApi.netEntity.Instantiate(netEntity);
    }

    private void OnNetStart()
    {
        Debug.Log("OnNetStart " + netEntity.netID);
    }
}
