using GalaxyCoreCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public static class Extensions  
{
    static Vector3 tmp = new Vector3();
    static Quaternion tmpQ = new Quaternion();
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
        tmp.x = vector.x;
        tmp.y = vector.y;
        tmp.z = vector.z;
        return tmp;
    }



    public static void Vector3(this GalaxyVector3 vector,out Vector3 vector3)
    {
        vector3.x = vector.x;
        vector3.y = vector.y;
        vector3.z = vector.z;        
    }

 

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
        if (quaternion == null) return UnityEngine.Quaternion.identity;
        tmpQ.x = quaternion.x;
        tmpQ.y = quaternion.y;
        tmpQ.z = quaternion.z;
        tmpQ.w = quaternion.w;
        return tmpQ;
    }
   
    public static void GalaxyInstantiate(this MonoBehaviour gameObject, GameObject prefab, Vector3 position, Quaternion rotation)
    {
       var go = GameObject.Instantiate(prefab,position,rotation);
        go.GetComponent<UnityNetEntity>();
        if (!go)
        {
            Debug.LogError("Префаб не имеет признаков сетевого объекта");
            GameObject.Destroy(go);
        }
    }
  

}
