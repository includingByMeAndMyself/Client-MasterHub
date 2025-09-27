using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;
using TestMaster.DataAccess;

namespace TestMaster.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MasterDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            // Регистрируем SQL генератор для SQLite
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }

        protected override void Seed(MasterDbContext context)
        {
            // Здесь можно добавить начальные данные
        }
    }
}
