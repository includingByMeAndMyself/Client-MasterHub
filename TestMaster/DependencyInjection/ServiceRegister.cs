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
            services.AddScoped<MasterDbContext>();
            services.AddScoped<IItemRepository, MasterItemRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<ISyncService, SyncService>();
            services.AddScoped<IItemManagementService, ItemManagementService>();
        }
    }
}
