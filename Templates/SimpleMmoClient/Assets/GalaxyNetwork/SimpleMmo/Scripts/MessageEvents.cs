using GalaxyCoreLib.Api;
 
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public  class MessageEvents  
{
    /*
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

    public delegate void DelegateOnWorldSync(MessageWorldSync message);
    /// <summary>
    /// Событие вызываемое при удалении объекта
    /// </summary>
    public event DelegateOnWorldSync OnWorldSync;

    public delegate void DelegateOnGoMessage(MessageGO message);
    /// <summary>
    /// Сообщение для объекта
    /// </summary>
    public event DelegateOnGoMessage OnGoMessage;

    public MessageEvents()
    {
        //Подписываемся на все входящие сообщения
        GalaxyEvents.OnGalaxyIncommingMessage += OnGalaxyIncommingMessage;
    }

    /// <summary>
    /// В этот метод приходят все входящии сообщения (мы на них подписались)
    /// </summary>
    /// <param name="code"></param>
    /// <param name="data"></param>
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
            case CommandType.worldSync:
                {
                    MessageWorldSync message = MessageWorldSync.Deserialize<MessageWorldSync>(data);
                    OnWorldSync?.Invoke(message);
                }
                break;
            case CommandType.goMessage:
                {  
                MessageGO message = MessageGO.Deserialize<MessageGO>(data);
                    OnGoMessage?.Invoke(message);
                }
                break;
        }
    }
    */
}
