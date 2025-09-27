using Microsoft.Extensions.DependencyInjection;
using TestMaster.DataAccess;
using TestMaster.Interfaces;
using TestMaster.Repositories;
using TestMaster.Services;

namespace TestMaster.DependencyInjection
{
    public static class ServiceRegister
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Регистрируем DbContext как Scoped
            services.AddScoped<MasterDbContext>();
            
            // Регистрируем репозиторий
            services.AddScoped<IItemRepository, MasterItemRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            // Регистрируем сервисы
            services.AddScoped<ISyncService, SyncService>();
        }
    }
}
