
using Common.Models;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TestMaster.Interfaces;
using System;

namespace TestMaster.Services
{
    internal class MasterHub : Hub
    {
        private readonly ISyncService _syncService;

        public MasterHub(ISyncService syncService)
        {
            _syncService = syncService;
        }

        public async Task<List<Item>> GetAllItems()
        {
            try
            {
                return await _syncService.GetLatestDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения данных: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task PushClientChanges(List<Item> clientChanges)
        {
            await _syncService.PushClientChangesAsync(clientChanges);
            var latest = await _syncService.GetLatestDataAsync();
            Clients.All.ReceiveServerUpdate(latest);
        }

        public async Task MasterUpdate(Item item)
        {
            await _syncService.MasterUpdateAsync(item);
            var latest = await _syncService.GetLatestDataAsync();
            Clients.All.ReceiveServerUpdate(latest);
        }
    }
}
