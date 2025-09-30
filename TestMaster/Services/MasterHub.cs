using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TestMaster.DataAccess;
using TestMaster.Repositories;

namespace TestMaster.Services
{
    public class MasterHub : Hub
    {
        public async Task<List<Item>> GetAllItems()
        {
            try
            {
                Console.WriteLine($"Получен запрос на получение всех элементов от клиента: {Context.ConnectionId}");

                using (var dbContext = new MasterDbContext())
                {
                    var itemRepo = new MasterItemRepository(dbContext);
                    var outboxRepo = new OutboxRepository(dbContext);
                    var syncService = new SyncService(itemRepo, outboxRepo);

                    var result = await syncService.GetLatestDataAsync();
                    Console.WriteLine($"Отправлено {result.Count} элементов клиенту {Context.ConnectionId}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения данных: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task PushClientChanges(List<Item> clientChanges)
        {
            try
            {
                Console.WriteLine($"Получены изменения от клиента {Context.ConnectionId}: {clientChanges.Count} элементов");

                using (var dbContext = new MasterDbContext())
                {
                    var itemRepo = new MasterItemRepository(dbContext);
                    var outboxRepo = new OutboxRepository(dbContext);
                    var syncService = new SyncService(itemRepo, outboxRepo);

                    await syncService.PushClientChangesAsync(clientChanges);
                    var latest = await syncService.GetLatestDataAsync();
                    Console.WriteLine($"Отправка обновлений всем клиентам: {latest.Count} элементов");
                    await Clients.All.ReceiveServerUpdate(latest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки изменений клиента: {ex.Message}");
            }
        }

        public async Task MasterUpdate(Item item)
        {
            try
            {
                Console.WriteLine($"Обновление мастера от клиента {Context.ConnectionId}: {item.Name}");

                using (var dbContext = new MasterDbContext())
                {
                    var itemRepo = new MasterItemRepository(dbContext);
                    var outboxRepo = new OutboxRepository(dbContext);
                    var syncService = new SyncService(itemRepo, outboxRepo);

                    await syncService.MasterUpdateAsync(item);
                    var latest = await syncService.GetLatestDataAsync();
                    Console.WriteLine($"Отправка обновлений всем клиентам: {latest.Count} элементов");
                    await Clients.All.ReceiveServerUpdate(latest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления мастера: {ex.Message}");
            }
        }

        public async Task ForceSync()
        {
            try
            {
                Console.WriteLine($"Принудительная синхронизация запрошена клиентом: {Context.ConnectionId}");

                using (var dbContext = new MasterDbContext())
                {
                    var itemRepo = new MasterItemRepository(dbContext);
                    var outboxRepo = new OutboxRepository(dbContext);
                    var syncService = new SyncService(itemRepo, outboxRepo);

                    var latest = await syncService.GetLatestDataAsync();
                    await Clients.All.ReceiveServerUpdate(latest);
                    Console.WriteLine($"Принудительная синхронизация: отправлено {latest.Count} элементов");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при принудительной синхронизации: {ex.Message}");
            }
        }

        public async Task NotifyClientsOfMasterChanges()
        {
            try
            {
                Console.WriteLine($"Уведомление клиентов запрошено: {Context.ConnectionId}");

                using (var dbContext = new MasterDbContext())
                {
                    var itemRepo = new MasterItemRepository(dbContext);
                    var outboxRepo = new OutboxRepository(dbContext);
                    var syncService = new SyncService(itemRepo, outboxRepo);

                    var latest = await syncService.GetLatestDataAsync();
                    await Clients.All.ReceiveServerUpdate(latest);
                    Console.WriteLine($"Уведомление отправлено {latest.Count} элементов всем подключенным клиентам");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при уведомлении клиентов: {ex.Message}");
            }
        }

        public override Task OnConnected()
        {
            Console.WriteLine($"Клиент подключен: {Context.ConnectionId}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine($"Клиент отключен: {Context.ConnectionId} (stopCalled: {stopCalled})");
            return base.OnDisconnected(stopCalled);
        }
    }
}