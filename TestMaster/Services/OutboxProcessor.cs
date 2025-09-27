using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestMaster.Interfaces;

namespace TestMaster.Services
{
    internal class OutboxProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private bool _isRunning;

        public OutboxProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            _isRunning = true;
            _timer = new Timer(ProcessOutboxMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Console.WriteLine("OutboxProcessor запущен");
        }

        public void Stop()
        {
            _isRunning = false;
            _timer?.Dispose();
            Console.WriteLine("OutboxProcessor остановлен");
        }

        private async void ProcessOutboxMessages(object state)
        {
            if (!_isRunning) return;

            try
            {
                // Создаем новый scope для каждой итерации
                using (var scope = _serviceProvider.CreateScope())
                {
                    var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                    var unprocessedMessages = await outboxRepository.GetUnprocessedAsync();

                    foreach (var message in unprocessedMessages)
                    {
                        await ProcessMessageAsync(message);
                        await outboxRepository.MarkAsProcessedAsync(message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки Outbox: {ex.Message}");
            }
        }

        private async Task ProcessMessageAsync(OutboxMessage message)
        {
            Console.WriteLine($"Обработка сообщения: {message.Type} - {message.Data}");
        }
    }
}
