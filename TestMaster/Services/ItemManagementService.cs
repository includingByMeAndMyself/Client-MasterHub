using System;
using System.Linq;
using System.Threading.Tasks;
using TestMaster.Interfaces;

namespace TestMaster.Services
{
    internal class ItemManagementService : IItemManagementService
    {
        private readonly ISyncService _syncService;

        public ItemManagementService(ISyncService syncService)
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        }

        public async Task ShowAllItemsAsync()
        {
            try
            {
                var items = await _syncService.GetLatestDataAsync();
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

        public async Task AddItemAsync()
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
                    ModifiedBy = "Master"
                };

                await _syncService.MasterUpdateAsync(item);
                Console.WriteLine("Элемент добавлен. Клиенты будут уведомлены через SignalR Hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении элемента: {ex.Message}");
            }
        }

        public async Task UpdateItemAsync()
        {
            try
            {
                Console.Write("Введите ID элемента для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный ID");
                    return;
                }

                var item = await _syncService.GetItemByIdAsync(id);
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
                item.ModifiedBy = "Master";

                await _syncService.MasterUpdateAsync(item);
                Console.WriteLine("Элемент обновлен. Клиенты будут уведомлены через SignalR Hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении элемента: {ex.Message}");
            }
        }

        public async Task DeleteItemAsync()
        {
            try
            {
                Console.Write("Введите ID элемента для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Неверный ID");
                    return;
                }

                var item = await _syncService.GetItemByIdAsync(id);
                if (item == null)
                {
                    Console.WriteLine("Элемент не найден");
                    return;
                }

                await _syncService.DeleteItemAsync(id);
                Console.WriteLine("Элемент удален. Клиенты будут уведомлены через SignalR Hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении элемента: {ex.Message}");
            }
        }

        public async Task ForceSyncWithClientsAsync()
        {
            try
            {
                Console.WriteLine("Принудительная синхронизация будет выполнена через SignalR Hub.");
                Console.WriteLine("Клиенты получат обновления при следующем взаимодействии с сервером.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при синхронизации: {ex.Message}");
            }
        }
    }
}