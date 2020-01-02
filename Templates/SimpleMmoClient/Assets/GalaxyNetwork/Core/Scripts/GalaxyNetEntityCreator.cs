using GalaxyCoreCommon.NetEntity;
using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyNetEntityCreator : MonoBehaviour
{
   
    void Start()
    {
        GalaxyApi.netEntity.OnGalaxyRemoteInstantiate += OnGalaxyRemoteInstantiate;
    }

    private GalaxyNetEntity OnGalaxyRemoteInstantiate(GalaxyNetEntity entity)
    {
        // UnityNetObject netGO = GameObject.Instantiate<UnityNetObject>(Resources.Load<UnityNetObject>(entity.name.Split(new char[] { ' ', '(' })[0]));
        Debug.Log(entity.name);
        UnityNetEntity go = GameObject.Instantiate<UnityNetEntity>(Resources.Load<UnityNetEntity>(entity.name));
        go.netId = entity.netID;
        Debug.Log(entity.transform.position.Vector3());
        if (entity.transform != null) {  
       // if (entity.transform.position != null)
      //  {
                go.transform.position = entity.transform.position.Vector3();
     //   }
     //   if (entity.transform.rotation != null)
     //   {
                go.transform.rotation = entity.transform.rotation.Quaternion();
     //   }
        }
        return null;
    }
}
