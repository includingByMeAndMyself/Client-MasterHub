using System;
using System.Threading.Tasks;
using TestMaster.Interfaces;
using TestMaster.Services;

namespace TestMaster
{
    public class Program
    {
        private static Startup _startup;
        private static SignalRHostedService _signalRService;
        private static OutboxProcessor _outboxProcessor;
        private static IItemManagementService _itemManagementService;

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Запуск TestMaster сервиса...");

                // Настраиваем сервисы
                _startup = new Startup();
                _startup.ConfigureServices();
                Console.WriteLine("Сервисы настроены успешно");

                // Получаем сервис для управления записями
                _itemManagementService = _startup.GetService<IItemManagementService>();
                Console.WriteLine("Сервис управления записями получен");

                Console.WriteLine("База данных SQLite инициализирована");

                // Запускаем SignalR сервер
                var signalRUrl = "http://localhost:8080";
                Console.WriteLine($"Запуск SignalR сервера на {signalRUrl}...");

                _signalRService = new SignalRHostedService(signalRUrl, _startup.ServiceProvider);
                _signalRService.Start();

                // Запускаем OutboxProcessor (без scope!)
                Console.WriteLine("Запуск OutboxProcessor...");
                _outboxProcessor = new OutboxProcessor(_startup.ServiceProvider);
                _outboxProcessor.Start();

                Console.WriteLine($"Мастер сервер запущен на {signalRUrl}");
                Console.WriteLine("\nДоступные команды:");
                Console.WriteLine("1 - Показать все элементы");
                Console.WriteLine("2 - Добавить элемент");
                Console.WriteLine("3 - Обновить элемент");
                Console.WriteLine("4 - Удалить элемент");
                Console.WriteLine("5 - Принудительная синхронизация с клиентами");
                Console.WriteLine("q - Выход");

                // Основной цикл
                while (true)
                {
                    var key = Console.ReadKey(true);

                    switch (key.KeyChar)
                    {
                        case '1':
                            await _itemManagementService.ShowAllItemsAsync();
                            break;
                        case '2':
                            await _itemManagementService.AddItemAsync();
                            break;
                        case '3':
                            await _itemManagementService.UpdateItemAsync();
                            break;
                        case '4':
                            await _itemManagementService.DeleteItemAsync();
                            break;
                        case '5':
                            await _itemManagementService.ForceSyncWithClientsAsync();
                            break;
                        case 'q':
                        case 'Q':
                            await StopServices();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске приложения: {ex.Message}");
                Console.WriteLine($"Детали: {ex}");
            }
            finally
            {
                await StopServices();
                Console.WriteLine("Сервис остановлен");
            }
        }

        private static async Task StopServices()
        {
            _signalRService?.Stop();
            _outboxProcessor?.Stop();
        }
    }
}