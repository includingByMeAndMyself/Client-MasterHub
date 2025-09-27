using System;
using System.Threading.Tasks;

namespace TestClient.Interfaces
{
    internal interface ISyncManager
    {
        Task StartAsync();
        Task StopAsync();
        Task SyncWithServerAsync();
        bool IsRunning { get; }
    }
}
