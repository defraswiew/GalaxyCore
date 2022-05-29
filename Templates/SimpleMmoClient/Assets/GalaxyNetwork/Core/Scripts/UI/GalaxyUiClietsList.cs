using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts.UI
{
    public class GalaxyUiClietsList : MonoBehaviour
    {
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        public GalaxyUiClientItem pref;
        private readonly Dictionary<int, GalaxyUiClientItem> items = new Dictionary<int, GalaxyUiClientItem>();
        private GalaxyConnection _connection;
        private void OnEnable()
        {
            _connection = GalaxyNetworkController.Api.MainConnection;
            _connection.Events.OnGalaxyIncomingClient += OnGalaxyIncomingClient;
            _connection.Events.OnGalaxyOutcomingClient += OnGalaxyOutcomingClient;
            _connection.Events.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            _connection.Events.OnGalaxyClientsUpdate += OnGalaxyClientsUpdate;

        }

        void OnDisable()
        {
            _connection.Events.OnGalaxyIncomingClient -= OnGalaxyIncomingClient;
            _connection.Events.OnGalaxyOutcomingClient -= OnGalaxyOutcomingClient;
            _connection.Events.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
            _connection.Events.OnGalaxyClientsUpdate -= OnGalaxyClientsUpdate;
        }


        private void OnGalaxyClientsUpdate()
        {
            Clear();
            foreach (var item in _connection.Api.Instances.Clients)
            {
                AddClient(item.Value);
            }
        }

        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            // чистим окно
            Clear();
            // запрашиваем новый список клиентов
            _connection.Api.Instances.ClientsUpdate();
        }
        /// <summary>
        /// Очистка окна от старых записей
        /// </summary>
        void Clear()
        {
            foreach (var item in items)
            {
                Destroy(item.Value.gameObject);
            }
            items.Clear();
        }
        /// <summary>
        /// Добавляем новую строку по классу клиента
        /// </summary>
        /// <param name="client"></param>
        private void AddClient(RemoteClient client)
        {
            if (items.ContainsKey(client.Id)) return;
            var uiClient = Instantiate(pref, content);
            uiClient.Init(client.Id, client.Name);
            items.Add(client.Id, uiClient);
        }
        private void OnGalaxyIncomingClient(RemoteClient client)
        {
            AddClient(client);
            Debug.Log("In " + client.Id + "  " + client.Name);
        }
        private void OnGalaxyOutcomingClient(RemoteClient client)
        {
            if (!items.ContainsKey(client.Id)) return;
            Debug.Log("Out " + client.Id + "  " + client.Name);
            Destroy(items[client.Id].gameObject);
            items.Remove(client.Id);
        }
    }
}