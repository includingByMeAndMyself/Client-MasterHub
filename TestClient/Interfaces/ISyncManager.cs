using System;
using System.Threading.Tasks;

namespace TestClient.Interfaces
{
    internal interface ISyncManager
    {
        Task StartAsync();
        void Stop();
        Task SyncWithServerAsync();
        Task ReconnectToServerAsync();
        void EnableAutoReconnect();
        void DisableAutoReconnect();
        void DisableAutoSync();
        bool IsRunning { get; }
        bool IsConnected { get; }
        bool IsAutoReconnectEnabled { get; }
    }
}