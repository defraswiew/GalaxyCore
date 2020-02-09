using GalaxyCoreCommon.NetEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public static class Extensions  
{

    public static GalaxyVector3 NetworkVector3(this Vector3 vector3)
    {
        GalaxyVector3 mes = new GalaxyVector3();
        mes.x = vector3.x;
        mes.y = vector3.y;
        mes.z = vector3.z;
        return mes;
    }
    /*
    public static GalaxyTransform NetworkTransform(this Transform transform)
    {
        GalaxyTransform  galaxyTransform = new GalaxyTransform();
        galaxyTransform.position = new GalaxyVector3();
        galaxyTransform.position = transform.position.NetworkVector3();
        galaxyTransform.rotation = new GalaxyQuaternion();
        galaxyTransform.rotation = transform.rotation.NetworkQuaternion();
        return galaxyTransform;
    }
    */
    

    public static Vector3 Vector3(this GalaxyVector3 vector)
    {
        Vector3 mes = new Vector3();
        mes.x = vector.x;
        mes.y = vector.y;
        mes.z = vector.z;
        return mes;
    }
    /*
    public static GalaxyQuaternion NetworkQuaternion(this Quaternion quaternion)
    {
        GalaxyQuaternion mes = new GalaxyQuaternion();
        mes.x = quaternion.x;
        mes.y = quaternion.y;
        mes.z = quaternion.z;
        mes.w = quaternion.w;
        return mes;
    }
    public static Quaternion Quaternion(this GalaxyQuaternion quaternion)
    {
        Quaternion mes = new Quaternion();
        mes.x = quaternion.x;
        mes.y = quaternion.y;
        mes.z = quaternion.z;
        mes.w = quaternion.w;
        return mes;
    }
   */

  

}
