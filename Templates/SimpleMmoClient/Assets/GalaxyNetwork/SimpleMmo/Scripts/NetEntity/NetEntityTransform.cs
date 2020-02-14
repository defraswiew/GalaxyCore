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
    MessageTransform messageTrandform;
    public bool sendPosition = true;
    private Vector3 randPos;
    public bool debug = false;
    Vector3 pos;
    void OnEnable()
    {
     //   Init();
            Invoke("Init", 1);
        InvokeRepeating("TEST", 5, 5);
    }

    private void Init()
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
                messageTrandform = BaseMessage.Deserialize<MessageTransform>(data);
                 
                UpdateTransform(messageTrandform);
                break;
        }
      
    }

    private void UpdateTransform(MessageTransform message)
    {
        // if (unityNetEntity.netEntity.isMy) return;

        if (message.position != null)
        {
            message.position.Vector3(out pos);
            transform.position = pos;
        }
        
        if (message.rotation != null) transform.rotation = message.rotation.Quaternion();
    }


    private void OnFrameUpdate()
    {
        if (!unityNetEntity.netEntity.isMy) return;
        if (!sendPosition) return;
        messageTrandform = new MessageTransform();
        messageTrandform.position = transform.position.NetworkVector3();
        messageTrandform.rotation = transform.rotation.NetworkQuaternion();
        unityNetEntity.netEntity.SendMessage((byte)NetEntityCommand.syncTransform, messageTrandform, GalaxyDeliveryType.unreliableNewest);
    }

    void TEST()
    {
        randPos.x = Random.Range(-5, 5);
        randPos.z = Random.Range(-5, 5);
    }
    /*
    private void Update()
    {
        if (!debug) return;
        if (!unityNetEntity) return;
        if(unityNetEntity.netEntity.isMy)
        transform.position = Vector3.Lerp(transform.position, randPos,Time.deltaTime);
    }
    */
}
