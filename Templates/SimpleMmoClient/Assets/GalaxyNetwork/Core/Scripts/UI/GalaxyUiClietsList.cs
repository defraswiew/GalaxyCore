using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System.Collections.Generic;
using UnityEngine;
namespace GalaxyCoreLib
{
    public class GalaxyUiClietsList : MonoBehaviour
    {
        /// <summary>
        /// Ссылка на контекс скрола
        /// </summary>
        [SerializeField]
        private RectTransform content;
        /// <summary>
        /// ссылка на префаб строки клиента
        /// </summary>
        [SerializeField]
        public GalaxyUiClientItem pref;
        /// <summary>
        /// текущий список строк
        /// </summary>
        private Dictionary<int, GalaxyUiClientItem> items = new Dictionary<int, GalaxyUiClientItem>();

        void OnEnable()
        {
            // вошел новый клиент
            GalaxyEvents.OnGalaxyIncomingClient += OnGalaxyIncomingClient;
            // вышел клиент
            GalaxyEvents.OnGalaxyOutcomingClient += OnGalaxyOutcomingClient;
            // мы вошли в комнату
            GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
            // пришел ответ на запрос списка клиентов
            GalaxyEvents.OnGalaxyClientsUpdate += OnGalaxyClientsUpdate;

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
            Clear();
            foreach (var item in GalaxyApi.instances.clients)
            {
                AddClient(item.Value);
            }
        }

        private void OnGalaxyEnterInInstance(InstanceInfo info)
        {
            // чистим окно
            Clear();
            // запрашиваем новый список клиентов
            GalaxyApi.instances.ClientsUpdate();
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
            if (items.ContainsKey(client.id)) return;
            GalaxyUiClientItem uiClient = Instantiate(pref, content);
            uiClient.Init(client.id, client.name);
            items.Add(client.id, uiClient);
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
}