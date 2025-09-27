using Microsoft.Extensions.DependencyInjection;
using System;
using TestMaster.Interfaces;
using TestMaster.Services;

namespace TestMaster
{
    public class Program
    {
        private static Startup _startup;
        private static SignalRHostedService _signalRService;
        private static OutboxProcessor _outboxProcessor;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Запуск TestMaster сервиса...");

                // Настраиваем сервисы
                _startup = new Startup();
                _startup.ConfigureServices();

                Console.WriteLine("Сервисы настроены успешно");
                Console.WriteLine("База данных SQLite инициализирована");

                // Запускаем SignalR сервер
                var signalRUrl = "http://localhost:8080";
                _signalRService = new SignalRHostedService(signalRUrl, _startup.ServiceProvider);
                _signalRService.Start();

                // Запускаем OutboxProcessor (без scope!)
                _outboxProcessor = new OutboxProcessor(_startup.ServiceProvider);
                _outboxProcessor.Start();

                Console.WriteLine($"Мастер сервер запущен на {signalRUrl}");
                Console.WriteLine("Нажмите 'q' для выхода...");

                // Ожидаем команду выхода
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске приложения: {ex.Message}");
                Console.WriteLine($"Детали: {ex}");
            }
            finally
            {
                _signalRService?.Stop();
                Console.WriteLine("Сервис остановлен");
            }
        }
    }
}
