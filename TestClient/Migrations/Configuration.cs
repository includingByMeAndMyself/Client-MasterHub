using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;
using TestClient.DataAccess;

namespace TestClient.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ClientDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            // Регистрируем SQL генератор для SQLite
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }

        protected override void Seed(ClientDbContext context)
        {
            // Здесь можно добавить начальные данные
        }
    }
}
