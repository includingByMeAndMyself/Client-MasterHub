using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;

namespace TestMaster.Extensions
{
    internal static class ModelBuilderExtensions
    {
        public static void RegisterConfigurations(this DbModelBuilder modelBuilder)
        {
            // Получаем все типы из текущей сборки
            var typesToRegister = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null &&
                              type.BaseType.IsGenericType &&
                              type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            // Регистрируем каждую конфигурацию
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
        }
    }
}
