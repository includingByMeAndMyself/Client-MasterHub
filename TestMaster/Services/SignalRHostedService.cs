using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using TestMaster.DependencyInjection;

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
            try
            {
                Console.WriteLine($"Запуск SignalR сервера на {_url}...");

                _webApp = WebApp.Start(_url, app =>
                {
                    // Настраиваем CORS для SignalR
                    app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

                    var hubConfig = new HubConfiguration
                    {
                        EnableDetailedErrors = true,
                        EnableJSONP = true,
                        //SResolver = new SignalRDependencyResolver(_serviceProvider)
                    };


                    app.MapSignalR("/signalr", hubConfig);

                    // Добавляем простой HTTP endpoint для проверки
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path.Value == "/")
                        {
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(@"
                                <html>
                                    <head><title>TestMaster SignalR Server</title></head>
                                    <body>
                                        <h1>TestMaster SignalR Server</h1>
                                        <p>Server now work on port 8080</p>
                                        <p>SignalR Hub: /signalr</p>
                                        <p>Status: <span style='color: green;'>Active</span></p>
                                    </body>
                                </html>
                            ");
                            return;
                        }
                        await next();
                    });
                });

                Console.WriteLine($"SignalR сервер успешно запущен на {_url}");
                Console.WriteLine($"SignalR Hub доступен по адресу: {_url}/signalr");
                Console.WriteLine($"HTTP endpoint доступен по адресу: {_url}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске SignalR сервера: {ex.Message}");
                Console.WriteLine($"Детали: {ex}");
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _webApp?.Dispose();
                Console.WriteLine("SignalR сервер остановлен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при остановке SignalR сервера: {ex.Message}");
            }
        }
    }
}