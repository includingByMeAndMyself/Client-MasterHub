using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestClient.Interfaces
{
    internal interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task AddOrUpdateAsync(Item item);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Item>> GetModifiedSinceAsync(DateTime lastSync);
        Task UpdateLastSyncTimeAsync();
        Task<DateTime?> GetLastSyncTimeAsync();
    }
}
