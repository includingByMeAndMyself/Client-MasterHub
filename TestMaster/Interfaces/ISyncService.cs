
using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestMaster.Interfaces
{
    internal interface ISyncService
    {
        Task<List<Item>> GetLatestDataAsync();
        Task PushClientChangesAsync(List<Item> clientChanges);
        Task MasterUpdateAsync(Item item);
    }
}
