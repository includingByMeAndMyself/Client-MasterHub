using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TestClient.DataAccess
{
    public class SyncInfo
    {
        public int Id { get; set; }
        public DateTime LastSyncTime { get; set; }
    }

    internal class SyncInfoConfiguration : EntityTypeConfiguration<SyncInfo>
    {
        public SyncInfoConfiguration()
        {
            // Настройка первичного ключа
            HasKey(e => e.Id);

            // Настройка свойства Id
            Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Настройка свойства LastSyncTime
            Property(e => e.LastSyncTime)
                .IsRequired();
        }
    }
}
