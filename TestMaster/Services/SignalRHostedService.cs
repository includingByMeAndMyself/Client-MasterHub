
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using System;

namespace TestMaster.Services
{
    internal class SignalRHostedService
    {
        private IDisposable _webApp;
        private readonly string _url;
        private readonly IServiceProvider _serviceProvider;

        public SignalRHostedService(string url, IServiceProvider serviceProvider)
        {
            _url = url;
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            _webApp = WebApp.Start(_url, app =>
            {
                var hubConfig = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    EnableJSONP = true,
                    Resolver = new SignalRDependencyResolver(_serviceProvider)
                };
                app.MapSignalR(hubConfig);
            });

            Console.WriteLine($"SignalR сервер запущен на {_url}");
        }

        public void Stop()
        {
            _webApp?.Dispose();
            Console.WriteLine("SignalR сервер остановлен");
        }
    }
}
