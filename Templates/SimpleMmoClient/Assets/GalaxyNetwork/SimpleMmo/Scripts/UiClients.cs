using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiClients : MonoBehaviour
{
    public GameObject button;
    public GameObject window;
    public GameObject content;
    public UiClientItem pref;

    private Dictionary<int, UiClientItem> items = new Dictionary<int, UiClientItem>();

    void OnEnable()
    {
        GalaxyEvents.OnGalaxyIncomingClient += OnGalaxyIncomingClient; // вошел новый клиент
        GalaxyEvents.OnGalaxyOutcomingClient += OnGalaxyOutcomingClient; // вышел клиент
        GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance; // мы вошли в комнату
        GalaxyEvents.OnGalaxyClientsUpdate += OnGalaxyClientsUpdate; // пришел ответ на запрос списка клиентов
         
    }

    void OnDisable()
    {
        GalaxyEvents.OnGalaxyIncomingClient -= OnGalaxyIncomingClient;
        GalaxyEvents.OnGalaxyOutcomingClient -= OnGalaxyOutcomingClient;
        GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
        GalaxyEvents.OnGalaxyClientsUpdate -= OnGalaxyClientsUpdate;
    }


    private void OnGalaxyClientsUpdate()
    {
        Debug.Log("OnGalaxyClientsUpdate");
        Clear();
        foreach (var item in GalaxyApi.instances.clients)
        {
            AddClient(item.Value);
        }
    }

    private void OnGalaxyEnterInInstance(GalaxyCoreCommon.InternalMessages.InstanceInfo info)
    {
        if (!window.activeSelf) button.SetActive(true);
        Clear();
    }

 
    void Clear()
    {
        foreach (var item in items)
        {
            Destroy(item.Value.gameObject);
        }
        items.Clear();
    }

    public void Draw(bool active)
    {
        if (active)
        {
            button.SetActive(false);
            window.SetActive(true);
            GalaxyApi.instances.ClientsUpdate(); // запрос списка клиентов
        } else
        {
            button.SetActive(true);
            window.SetActive(false);
        }
       
    }

    private void AddClient(RemoteClient client)
    {
        if (items.ContainsKey(client.id)) return;
        UiClientItem uiClient = Instantiate(pref, content.transform);
        uiClient.Set(client.id, client.name);
        items.Add(client.id,uiClient);
    }


    private void OnGalaxyIncomingClient(RemoteClient client)
    {       
        AddClient(client);
        Debug.Log("Вошел " + client.id + "  " + client.name);
    }
    private void OnGalaxyOutcomingClient(RemoteClient client)
    {
        if (!items.ContainsKey(client.id)) return;
        Debug.Log("Вышел " + client.id + "  " + client.name);
        Destroy(items[client.id].gameObject);
    }
}
