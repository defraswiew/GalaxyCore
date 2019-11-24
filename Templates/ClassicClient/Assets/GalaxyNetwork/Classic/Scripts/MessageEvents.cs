using GalaxyCoreLib.Api;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public  class MessageEvents  
{
    public delegate void DelegateOnMessInstantiate(MessageInstantiate message);
    public event DelegateOnMessInstantiate OnMessInstantiate;

    public delegate void DelegateOnMessTransform(MessageTransform message);
    public event DelegateOnMessTransform OnMessTransform;

    public MessageEvents()
    {
        GalaxyEvents.OnGalaxyIncommingMessage += OnGalaxyIncommingMessage;
    }

    private void OnGalaxyIncommingMessage(byte code, byte[] data)
    {
        switch ((CommandType)code)
        {
            case CommandType.goInstantiate:
                { 
                MessageInstantiate message = MessageInstantiate.Deserialize<MessageInstantiate>(data);
                OnMessInstantiate?.Invoke(message);
                }
                break;
            case CommandType.goTransform:
                {
                    MessageTransform message = MessageTransform.Deserialize<MessageTransform>(data);
                    OnMessTransform?.Invoke(message);
                }
                break;

        }
    }
}
