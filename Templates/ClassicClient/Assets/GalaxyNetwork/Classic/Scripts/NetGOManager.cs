using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyTemplateCommon;
using GalaxyTemplateCommon.Messages;
using System.Linq;
public class NetGOManager
{
    int localId = 0;

    public Dictionary<int, NetGO> netGOs = new Dictionary<int, NetGO>();
    public List<NetGO> netGOsNoSync = new List<NetGO>();

    public NetGOManager()
    {
        //Подписываемся на сообщение о создании нового объекта
        StaticLinks.messageEvents.OnMessInstantiate += OnMessInstantiate;
        //Подписываемся на сообщение о смене параметров трансформа
        StaticLinks.messageEvents.OnMessTransform += OnMessTransform;
        //Подписываемся на сообщение о удалении объекта
        StaticLinks.messageEvents.OnMessDestroy += OnMessDestroy;
        //Подписываемся на сообщение о синхронизации мира
        StaticLinks.messageEvents.OnWorldSync += OnWorldSync;
    }

    private void OnWorldSync(MessageWorldSync message)
    {
        foreach (var item in message.netObjects)
        {
            OnMessInstantiate(item);
        }
    }

    private void OnMessDestroy(MessageDestroyGO message)
    {
        if (netGOs.ContainsKey(message.netID))
        {
            GameObject.Destroy(netGOs[message.netID].gameObject);              
        }
    }

    private void OnMessTransform(MessageTransform message)
    {
        if (!netGOs.ContainsKey(message.netID)) return;
        NetGO go = netGOs[message.netID];
        if (go.isMy) return;
        if(message.position!=null) go.transform.position = message.position.Vector3();
        if (message.rotation != null) go.transform.rotation = message.rotation.Quaternion();
    }

    private void OnMessInstantiate(MessageInstantiate message)
    {
        Debug.Log("Создан объект ID:" + message.netID);
        NetGO netGO;
        //проверяем наш ли это объект
       if (message.owner == StaticLinks.clientData.myId)
        {
            //ищем в коллекции несинхронизированных объектов тот который успешно создался
            netGO = netGOsNoSync.Where(x => x.localId == message.localId).FirstOrDefault();
            //если почему то не нашли то выходим
            if (netGO == null) return;
            //если всетаки нашли то инициализируем, и переводим в правильный список
            netGO.isMy = true; //помечаем что он наш         
            netGOsNoSync.Remove(netGO); // Удаляем его из временного списка         
        } else
        {
            //а если он не наш, то просто создаем его
            netGO = GameObject.Instantiate<NetGO>(Resources.Load<NetGO>(message.name.Split(' ')[0]));
            netGO.isMy = false;
            
            netGO.transform.position = message.position.Vector3();
            netGO.transform.rotation = message.rotation.Quaternion();
        }
        netGO.netID = message.netID;
        netGO.sync = true; //помечаем что объект синхронизирован с сервером
        netGOs.Add(netGO.netID,netGO);// Добавляем в нормальный словарь
    }

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
        netGOsNoSync.Add(netGO);
    }

}
