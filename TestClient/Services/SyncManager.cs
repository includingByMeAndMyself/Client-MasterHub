using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestClient.Interfaces;
using TestClient.Strategies;

namespace TestClient.Services
{
    internal class SyncManager : ISyncManager
    {
        private readonly ILocalItemRepository _localRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IServerProxy _serverProxy;
        private readonly IConflictResolutionStrategy _conflictStrategy;
        private readonly Timer _syncTimer;
        private readonly Timer _reconnectTimer;
        private readonly int _syncInterval;
        private readonly int _reconnectInterval;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public SyncManager(
            ILocalItemRepository localRepository,
            IOutboxRepository outboxRepository,
            IServerProxy serverProxy,
            IConflictResolutionStrategy conflictStrategy)
        {
            _localRepository = localRepository;
            _outboxRepository = outboxRepository;
            _serverProxy = serverProxy;
            _conflictStrategy = conflictStrategy;
            _syncInterval = int.Parse(ConfigurationManager.AppSettings["SyncInterval"]);
            _reconnectInterval = int.Parse(ConfigurationManager.AppSettings["ReconnectInterval"]);

            _syncTimer = new Timer(SyncTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            _reconnectTimer = new Timer(ReconnectTimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            // Подписываемся на события сервера
            _serverProxy.OnServerUpdate += OnServerUpdate;
            _serverProxy.OnConnected += OnServerConnected;
            _serverProxy.OnDisconnected += OnServerDisconnected;
        }

        public async Task StartAsync()
        {
            if (_isRunning) return;

            _isRunning = true;
            Console.WriteLine("Запуск менеджера синхронизации...");

            // Подключаемся к серверу
            await _serverProxy.ConnectAsync();

            // Запускаем таймер синхронизации
            _syncTimer.Change(0, _syncInterval);

            // Запускаем таймер переподключения
            _reconnectTimer.Change(_reconnectInterval, _reconnectInterval);
        }

        public async Task StopAsync()
        {
            if (!_isRunning) return;

            _isRunning = false;
            Console.WriteLine("Остановка менеджера синхронизации...");

            _syncTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);

            await _serverProxy.DisconnectAsync();
        }

        public async Task SyncWithServerAsync()
        {
            if (!_serverProxy.IsConnected)
            {
                Console.WriteLine("Сервер не подключен, пропускаем синхронизацию");
                return;
            }

            try
            {
                Console.WriteLine("Начинаем синхронизацию...");

                // Получаем данные с сервера
                var serverItems = await _serverProxy.GetAllItemsAsync();
                var localItems = await _localRepository.GetAllAsync();

                // Разрешаем конфликты
                var resolvedItems = _conflictStrategy.ResolveConflicts(serverItems, localItems.ToList());

                // Обновляем локальную базу
                foreach (var item in resolvedItems)
                {
                    await _localRepository.AddOrUpdateAsync(item);
                }

                // Отправляем локальные изменения на сервер
                var lastSync = await _localRepository.GetLastSyncTimeAsync();
                if (lastSync.HasValue)
                {
                    var localChanges = await _localRepository.GetModifiedSinceAsync(lastSync.Value);
                    if (localChanges.Any())
                    {
                        await _serverProxy.PushClientChangesAsync(localChanges.ToList());
                    }
                }

                // Обрабатываем исходящие сообщения
                await ProcessOutboxMessages();

                // Обновляем время последней синхронизации
                await _localRepository.UpdateLastSyncTimeAsync();

                Console.WriteLine($"Синхронизация завершена. Обработано {resolvedItems.Count} элементов");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при синхронизации: {ex.Message}");
            }
        }

        private async void SyncTimerCallback(object state)
        {
            if (_isRunning)
            {
                await SyncWithServerAsync();
            }
        }

        private async void ReconnectTimerCallback(object state)
        {
            if (_isRunning && !_serverProxy.IsConnected)
            {
                Console.WriteLine("Попытка переподключения к серверу...");
                await _serverProxy.ConnectAsync();
            }
        }

        private async void OnServerUpdate(List<Item> items)
        {
            Console.WriteLine($"Получено обновление с сервера: {items.Count} элементов");
            
            // Обновляем локальную базу
            foreach (var item in items)
            {
                await _localRepository.AddOrUpdateAsync(item);
            }
        }

        private void OnServerConnected()
        {
            Console.WriteLine("Подключение к серверу установлено");
        }

        private void OnServerDisconnected()
        {
            Console.WriteLine("Соединение с сервером потеряно");
        }

        private async Task ProcessOutboxMessages()
        {
            var unprocessedMessages = await _outboxRepository.GetUnprocessedAsync();
            
            foreach (var message in unprocessedMessages)
            {
                try
                {
                    if (message.Type == "ItemUpdate")
                    {
                        var item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(message.Data);
                        await _serverProxy.PushClientChangesAsync(new List<Item> { item });
                    }
                    
                    await _outboxRepository.MarkAsProcessedAsync(message.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки исходящего сообщения {message.Id}: {ex.Message}");
                }
            }

            // Удаляем обработанные сообщения
            await _outboxRepository.DeleteProcessedAsync();
        }
    }
}
