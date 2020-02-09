using GalaxyCoreCommon;
using GalaxyCoreCommon.NetEntity;
using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityNetObject : GalaxyNetEntity
{ 

    /// <summary>
    /// Отправить сетевому объекту сообщение
    /// </summary>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <param name="deliveryType"></param>
    public void SendMessage(byte code,byte[] data, GalaxyDeliveryType deliveryType = GalaxyDeliveryType.unreliable)
    {
        GalaxyApi.send.SendMessageToNetEntity(netID, code, data, deliveryType);
    }
    /// <summary>
    /// Отправить сетевому объекту сообщение
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="deliveryType"></param>
    public void SendMessage(byte code, BaseMessage message, GalaxyDeliveryType deliveryType = GalaxyDeliveryType.unreliable)
    {
        GalaxyApi.send.SendMessageToNetEntity(netID, code, message.Serialize(), deliveryType);
    }
}
