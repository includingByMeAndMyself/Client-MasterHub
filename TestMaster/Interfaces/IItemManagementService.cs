using System.Threading.Tasks;

namespace TestMaster.Interfaces
{
    public interface IItemManagementService
    {
        Task ShowAllItemsAsync();
        Task AddItemAsync();
        Task UpdateItemAsync();
        Task DeleteItemAsync();
        Task ForceSyncWithClientsAsync();
    }
}