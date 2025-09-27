using System;
using Microsoft.Extensions.DependencyInjection;
using TestClient.DataAccess;
using TestClient.DependencyInjection;

namespace TestClient
{
    public class Startup
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public void ConfigureServices()
        {
            // Создаем коллекцию сервисов
            var services = new ServiceCollection();
            
            // Регистрируем сервисы
            ServiceRegister.RegisterServices(services);
            
            // Создаем провайдер сервисов
            ServiceProvider = services.BuildServiceProvider();
            
            // Инициализируем базу данных
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ClientDbContext>();
                    // Проверяем, существует ли база данных, и создаем ее если нужно
                    context.Database.Initialize(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
                throw;
            }
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        public IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }
    }
}
