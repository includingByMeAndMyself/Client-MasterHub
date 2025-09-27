using System;
using System.Threading.Tasks;
using TestClient.Interfaces;

namespace TestClient
{
    internal class Program
    {
        private static ISyncManager _syncManager;
        private static ILocalItemRepository _localRepository;
        private static IOutboxRepository _outboxRepository;
        private static IServerProxy _serverProxy;
        private static Startup _startup;

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Запуск TestClient...");

                // Настраиваем сервисы
                _startup = new Startup();
                _startup.ConfigureServices();

                // Получаем сервисы
                _localRepository = _startup.GetService<ILocalItemRepository>();
                _outboxRepository = _startup.GetService<IOutboxRepository>();
                _serverProxy = _startup.GetService<IServerProxy>();
                _syncManager = _startup.GetService<ISyncManager>();

                // Запускаем синхронизацию
                await _syncManager.StartAsync();

                Console.WriteLine("Клиент запущен. Доступные команды:");
                Console.WriteLine("1 - Показать все элементы");
                Console.WriteLine("2 - Добавить элемент");
                Console.WriteLine("3 - Обновить элемент");
                Console.WriteLine("4 - Удалить элемент");
                Console.WriteLine("5 - Принудительная синхронизация");
                Console.WriteLine("q - Выход");

                // Основной цикл
                while (true)
                {
                    var key = Console.ReadKey(true);
                    
                    switch (key.KeyChar)
                    {
                        case '1':
                            await ShowAllItems();
                            break;
                        case '2':
                            await AddItem();
                            break;
                        case '3':
                            await UpdateItem();
                            break;
                        case '4':
                            await DeleteItem();
                            break;
                        case '5':
                            await _syncManager.SyncWithServerAsync();
                            break;
                        case 'q':
                        case 'Q':
                            await _syncManager.StopAsync();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске клиента: {ex.Message}");
                Console.WriteLine($"Детали: {ex}");
            }
            finally
            {
                _syncManager?.StopAsync().Wait();
                Console.WriteLine("Клиент остановлен");
            }
        }

        private static async Task ShowAllItems()
        {
            try
            {
                var items = await _localRepository.GetAllAsync();
                Console.WriteLine($"\nВсего элементов: {items.Count()}");
                foreach (var item in items)
                {
                    Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Modified: {item.LastModified}, By: {item.ModifiedBy}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении элементов: {ex.Message}");
            }
        }

        private static async Task AddItem()
        {
            try
            {
                Console.Write("Введите имя элемента: ");
                var name = Console.ReadLine();
                
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Имя не может быть пустым");
                    return;
                }

                var item = new Common.Models.Item
                {
                    Id = 0, // Будет установлен автоматически
                    Name = name,
                    LastModified = DateTime.UtcNow,
                    ModifiedBy = "Client"
                };

                await _localRepository.AddOrUpdateAsync(item);
                
                // Добавляем в outbox для отправки на сервер
                await _outboxRepository.AddAsync(new Common.Models.OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = "ItemUpdate",
                    Data = Newtonsoft.Json.JsonConvert.SerializeObject(item),
                    CreatedAt = DateTime.UtcNow,
                    Processed = false
                });

                Console.WriteLine("Элемент добавлен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении элемента: {ex.Message}");
            }
        }

        private static async Task UpdateItem()
        {
            try
            {
                Console.Write("Введите ID элемента для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный ID");
                    return;
                }

                var item = await _localRepository.GetByIdAsync(id);
                if (item == null)
                {
                    Console.WriteLine("Элемент не найден");
                    return;
                }

                Console.Write($"Текущее имя: {item.Name}. Введите новое имя: ");
                var newName = Console.ReadLine();
                
                if (string.IsNullOrEmpty(newName))
                {
                    Console.WriteLine("Имя не может быть пустым");
                    return;
                }

                item.Name = newName;
                item.LastModified = DateTime.UtcNow;
                item.ModifiedBy = "Client";

                await _localRepository.AddOrUpdateAsync(item);
                
                // Добавляем в outbox для отправки на сервер
                await _outboxRepository.AddAsync(new Common.Models.OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = "ItemUpdate",
                    Data = Newtonsoft.Json.JsonConvert.SerializeObject(item),
                    CreatedAt = DateTime.UtcNow,
                    Processed = false
                });

                Console.WriteLine("Элемент обновлен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении элемента: {ex.Message}");
            }
        }

        private static async Task DeleteItem()
        {
            try
            {
                Console.Write("Введите ID элемента для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный ID");
                    return;
                }

                var item = await _localRepository.GetByIdAsync(id);
                if (item == null)
                {
                    Console.WriteLine("Элемент не найден");
                    return;
                }

                await _localRepository.DeleteAsync(id);
                Console.WriteLine("Элемент удален");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении элемента: {ex.Message}");
            }
        }
    }
}
