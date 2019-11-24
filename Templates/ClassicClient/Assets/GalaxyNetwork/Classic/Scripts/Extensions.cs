using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
public static class Extensions  
{
    public static MessageVector3 NetworkVector3(this Vector3 vector3)
    {
        MessageVector3 mes = new MessageVector3();
        mes.x = vector3.x;
        mes.y = vector3.y;
        mes.z = vector3.z;
        return mes;
    }
    public static MessageQuaternion NetworkQuaternion(this Quaternion quaternion)
    {
        MessageQuaternion mes = new MessageQuaternion();
        mes.x = quaternion.x;
        mes.y = quaternion.y;
        mes.z = quaternion.z;
        mes.w = quaternion.w;
        return mes;
    }
}
