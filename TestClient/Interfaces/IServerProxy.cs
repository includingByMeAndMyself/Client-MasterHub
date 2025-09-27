using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestClient.Interfaces
{
    internal interface IServerProxy
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        Task<List<Item>> GetAllItemsAsync();
        Task PushClientChangesAsync(List<Item> changes);
        bool IsConnected { get; }
        event Action<List<Item>> OnServerUpdate;
        event Action OnConnected;
        event Action OnDisconnected;
    }
}
