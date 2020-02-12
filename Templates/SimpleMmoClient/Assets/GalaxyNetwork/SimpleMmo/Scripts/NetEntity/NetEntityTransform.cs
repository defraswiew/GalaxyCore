using GalaxyCoreCommon;
using GalaxyCoreLib.Api;
using SimpleMmoCommon;
using SimpleMmoCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetEntityTransform : MonoBehaviour
{
    UnityNetEntity unityNetEntity;

    void Start()
    {
        unityNetEntity = GetComponent<UnityNetEntity>();
        unityNetEntity.netEntity.OnInMessage += OnInMessage;
        GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
    }

    private void OnDestroy()
    {
        GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
    }

    private void OnInMessage(byte code, byte[] data)
    {
        switch ((NetEntityCommand)code)
        {
            case NetEntityCommand.syncTransform:
                MessageTransform message = BaseMessage.Deserialize<MessageTransform>(data);
                UpdateTransform(message);
                break;
        }
      
    }

    private void UpdateTransform(MessageTransform message)
    {
        if (unityNetEntity.netEntity.isMy) return;
        if (message.position != null) transform.position = message.position.Vector3();
        if (message.rotation != null) transform.rotation = message.rotation.Quaternion();
    }


    private void OnFrameUpdate()
    {
        if (!unityNetEntity.netEntity.isMy) return;
        MessageTransform message = new MessageTransform();
        message.position = transform.position.NetworkVector3();
        message.rotation = transform.rotation.NetworkQuaternion();
        unityNetEntity.netEntity.SendMessage((byte)NetEntityCommand.syncTransform, message, GalaxyDeliveryType.unreliableNewest);
    }


}
