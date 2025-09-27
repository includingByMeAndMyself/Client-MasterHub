using Common.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TestClient.Interfaces;

namespace TestClient.Proxies
{
    internal class ServerHubProxy : IServerProxy
    {
        private HubConnection _connection;
        private IHubProxy _hubProxy;
        private readonly string _serverUrl;

        public bool IsConnected => _connection?.State == ConnectionState.Connected;

        public event Action<List<Item>> OnServerUpdate;
        public event Action OnConnected;
        public event Action OnDisconnected;

        public ServerHubProxy()
        {
            _serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _connection = new HubConnection(_serverUrl);
                _hubProxy = _connection.CreateHubProxy("MasterHub");

                // Подписываемся на события
                _hubProxy.On<List<Item>>("ReceiveServerUpdate", (items) =>
                {
                    OnServerUpdate?.Invoke(items);
                });

                _connection.StateChanged += (change) =>
                {
                    if (change.NewState == ConnectionState.Connected)
                    {
                        OnConnected?.Invoke();
                    }
                    else if (change.NewState == ConnectionState.Disconnected)
                    {
                        OnDisconnected?.Invoke();
                    }
                };

                await _connection.Start();
                return IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.Stop();
                _connection.Dispose();
                _connection = null;
            }
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            try
            {
                return await _hubProxy.Invoke<List<Item>>("GetAllItems");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения данных с сервера: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task PushClientChangesAsync(List<Item> changes)
        {
            try
            {
                await _hubProxy.Invoke("PushClientChanges", changes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки изменений на сервер: {ex.Message}");
            }
        }
    }
}
