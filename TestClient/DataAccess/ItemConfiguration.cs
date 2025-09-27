using Common.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TestClient.DataAccess
{
    internal class ItemConfiguration : EntityTypeConfiguration<Item>
    {
        public ItemConfiguration()
        {
            // Настройка первичного ключа
            HasKey(e => e.Id);

            // Настройка свойства Id
            Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Настройка свойства Name
            Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            // Настройка свойства LastModified
            Property(e => e.LastModified)
                .IsRequired();

            // Настройка свойства ModifiedBy
            Property(e => e.ModifiedBy)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
