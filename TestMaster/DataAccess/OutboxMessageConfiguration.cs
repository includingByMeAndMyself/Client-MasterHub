using Common.Models;
using System.Data.Entity.ModelConfiguration;

namespace TestMaster.DataAccess
{
    internal class OutboxMessageConfiguration : EntityTypeConfiguration<OutboxMessage>
    {
        public OutboxMessageConfiguration()
        {
            // Настройка первичного ключа
            HasKey(e => e.Id);

            // Настройка свойства Type
            Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);

            // Настройка свойства Data
            Property(e => e.Data)
                .IsRequired();

            // Настройка свойства CreatedAt
            Property(e => e.CreatedAt)
                .IsRequired();

            // Настройка свойства Processed
            Property(e => e.Processed)
                .IsRequired();
        }
    }
}
