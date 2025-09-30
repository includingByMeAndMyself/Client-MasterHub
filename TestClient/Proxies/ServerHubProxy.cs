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
            Console.WriteLine($"Настроен URL сервера: {_serverUrl}");
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                Console.WriteLine($"Попытка подключения к серверу: {_serverUrl}");

                // В SignalR для ASP.NET клиент подключается к базовому URL сервера
                // SignalR автоматически найдет Hub по имени
                _connection = new HubConnection(_serverUrl);
                _hubProxy = _connection.CreateHubProxy("MasterHub");

                // Подписываемся на события
                _hubProxy.On<List<Item>>("ReceiveServerUpdate", (items) =>
                {
                    Console.WriteLine($"Получено обновление с сервера: {items.Count} элементов");
                    OnServerUpdate?.Invoke(items);
                });

                _connection.StateChanged += (change) =>
                {
                    Console.WriteLine($"Изменение состояния подключения: {change.OldState} -> {change.NewState}");
                    if (change.NewState == ConnectionState.Connected)
                    {
                        Console.WriteLine("Подключение к серверу установлено");
                        OnConnected?.Invoke();
                    }
                    else if (change.NewState == ConnectionState.Disconnected)
                    {
                        Console.WriteLine("Соединение с сервером потеряно");
                        OnDisconnected?.Invoke();
                    }
                };

                // Добавляем обработку ошибок
                _connection.Error += (error) =>
                {
                    Console.WriteLine($"Ошибка SignalR: {error.Message}");
                };

                Console.WriteLine("Запуск подключения...");
                await _connection.Start();

                if (IsConnected)
                {
                    Console.WriteLine("Успешно подключен к серверу");
                }
                else
                {
                    Console.WriteLine("Не удалось подключиться к серверу");
                }

                return IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
                Console.WriteLine($"Тип ошибки: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_connection != null)
                {
                    Console.WriteLine("Отключение от сервера...");
                    _connection.Stop();
                    _connection.Dispose();
                    _connection = null;
                    Console.WriteLine("Отключение завершено");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отключении: {ex.Message}");
            }
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            try
            {
                if (!IsConnected)
                {
                    Console.WriteLine("Сервер не подключен, невозможно получить данные");
                    return new List<Item>();
                }

                Console.WriteLine("Запрос данных с сервера...");
                var result = await _hubProxy.Invoke<List<Item>>("GetAllItems");
                Console.WriteLine($"Получено {result.Count} элементов с сервера");
                return result;
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
                if (!IsConnected)
                {
                    Console.WriteLine("Сервер не подключен, невозможно отправить изменения");
                    return;
                }

                Console.WriteLine($"Отправка {changes.Count} изменений на сервер...");
                await _hubProxy.Invoke("PushClientChanges", changes);
                Console.WriteLine("Изменения успешно отправлены на сервер");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки изменений на сервер: {ex.Message}");
            }
        }
    }
}