using System;
using System.Threading.Tasks;

namespace TestClient.Interfaces
{
    internal interface ISyncManager
    {
        Task StartAsync();
        void Stop();
        Task SyncWithServerAsync();
        bool IsRunning { get; }
    }
}
