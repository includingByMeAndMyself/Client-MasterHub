using Microsoft.Extensions.DependencyInjection;
using TestClient.DataAccess;
using TestClient.Interfaces;
using TestClient.Proxies;
using TestClient.Repositories;
using TestClient.Services;
using TestClient.Strategies;

namespace TestClient.DependencyInjection
{
    public static class ServiceRegister
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Регистрируем DbContext как Scoped
            services.AddScoped<ClientDbContext>();
            
            // Регистрируем репозитории
            services.AddScoped<IItemRepository, ClientItemRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            // Регистрируем стратегии
            services.AddScoped<IConflictResolutionStrategy, LastWriteWinsStrategy>();

            // Регистрируем сервисы
            services.AddScoped<IServerProxy, ServerHubProxy>();
            services.AddScoped<ISyncManager, SyncManager>();
        }
    }
}
