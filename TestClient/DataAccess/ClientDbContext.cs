using System;
using System.Data.Common;
using System.Data.Entity;
using Common.Models;

namespace TestClient.DataAccess
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext() : base("LocalConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ClientDbContext>());
        }

        public ClientDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ClientDbContext>());
        }

        public ClientDbContext(DbConnection connection) : base(connection, true)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ClientDbContext>());
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<SyncInfo> SyncInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RegisterConfigurations();
        }
    }
}
