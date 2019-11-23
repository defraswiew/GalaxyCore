using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using GalaxyCoreServer.Api;

namespace GalaxyTemplate.Clients
{
   public class ClientManager
    {
        /// <summary>
        /// Список клиентов по нет конекшену
        /// </summary>
        ConcurrentDictionary<ClientConnection, Client> clients = new ConcurrentDictionary<ClientConnection, Client>();

        /// Список клиентов по ид
        /// </summary>
        ConcurrentDictionary<int, Client> clientsID = new ConcurrentDictionary<int, Client>();
        
        public ClientManager()
        {
            GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect; // Подписываемся на событие дисконекта клиента
        }

        private void OnGalaxyDisconnect(ClientConnection clientConnection)
        {
            RemoveClient(clientConnection);
        }

        /// <summary>
        /// Добавить клиента
        /// </summary>
        /// <param name="client"></param>
        public void AddClient(Client client)
        {
            clients.TryAdd(client.clientConnection, client);
            clientsID.TryAdd(client.id, client);
            if (Server.debugLog) Console.WriteLine("Добавлен новый клиент ид:"+ client.id);
        }
        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="connection"></param>
        public void RemoveClient(ClientConnection connection)
        {
            Client client;
            clients.TryRemove(connection, out client);
            clientsID.TryRemove(client.id, out client);
            if (Server.debugLog) Console.WriteLine("Клиент ID:" + client.id + " решил нас покинуть");
        }

        /// <summary>
        /// Получить клиента по подключению
        /// </summary>
        /// <param name="clientConnection"></param>
        /// <returns></returns>
        public Client GetClientByConnection(ClientConnection clientConnection)
        {
            return clients[clientConnection];
        }


    }
}
