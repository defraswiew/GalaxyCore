using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
public class NetGOManager
{
    int localId = 0;

    private int GetNewLocalId()
    {
        localId++;
        return localId;
    }

    public void NetInstantiate(NetGO netGO)
    {
        MessageInstantiate message = new MessageInstantiate();
        message.name = netGO.name;
        message.localId = GetNewLocalId();
        message.position = netGO.transform.position.NetworkVector3();
        message.rotation = netGO.transform.rotation.NetworkQuaternion();
        netGO.localId = message.localId;
        GalaxyApi.send.SendMessageToServer((byte)CommandType.goInstantiate, message, GalaxyCoreCommon.GalaxyDeliveryType.reliable);
    }

}
