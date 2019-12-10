using GalaxyCoreLib.Api;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public  class MessageEvents  
{
    public delegate void DelegateOnMessInstantiate(MessageInstantiate message);
    /// <summary>
    /// Событие вызываемое при создании объекта
    /// </summary>
    public event DelegateOnMessInstantiate OnMessInstantiate;

    public delegate void DelegateOnMessTransform(MessageTransform message);
    /// <summary>
    /// Событие вызываемое при перемещении объекта
    /// </summary>
    public event DelegateOnMessTransform OnMessTransform;

    public delegate void DelegateOnMessDestroy(MessageDestroyGO message);
    /// <summary>
    /// Событие вызываемое при удалении объекта
    /// </summary>
    public event DelegateOnMessDestroy OnMessDestroy;

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
            case CommandType.goDestroy:
                {
                    MessageDestroyGO message = MessageDestroyGO.Deserialize<MessageDestroyGO>(data);
                    OnMessDestroy?.Invoke(message);
                }
                break;


        }
    }
}
