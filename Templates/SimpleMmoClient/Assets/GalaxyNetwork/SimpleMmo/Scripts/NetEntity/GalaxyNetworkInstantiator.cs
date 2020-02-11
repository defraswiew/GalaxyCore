using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
public class GalaxyNetworkInstantiator : MonoBehaviour
{
    private void OnEnable()
    {
        GalaxyEvents.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
    }

    private void OnDisable()
    {
        GalaxyEvents.OnGalaxyNetEntityInstantiate -= OnGalaxyNetEntityInstantiate;
    }

    private void OnGalaxyNetEntityInstantiate(ClientNetEntity netEntity)
    {
        Debug.Log(netEntity.name + " " + netEntity.position.Vector3());
        if (netEntity.name == "") return;
        UnityNetEntity go = Instantiate(Resources.Load<UnityNetEntity>(netEntity.name), netEntity.position.Vector3(), netEntity.rotation.Quaternion());
        go.netEntity = netEntity;
       // if (netEntity.position != null) go.transform.position = go.netEntity.position.Vector3();
       // if (go.netEntity.rotation != null) go.transform.rotation = go.netEntity.rotation.Quaternion();         
    }
}
