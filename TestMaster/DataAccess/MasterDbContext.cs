using System;
using System.Data.Common;
using System.Data.Entity;
using Common.Models;
using TestMaster.Extensions;

namespace TestMaster.DataAccess
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext() : base("DefaultConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<MasterDbContext>());
        }

        public MasterDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<MasterDbContext>());
        }

        public MasterDbContext(DbConnection connection) : base(connection, true)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<MasterDbContext>());
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RegisterConfigurations();
        }
    }
}
