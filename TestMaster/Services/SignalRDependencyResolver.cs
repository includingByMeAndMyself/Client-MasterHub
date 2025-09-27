using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace TestMaster.Services
{
    internal class SignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public SignalRDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType) ?? base.GetServices(serviceType);
        }
    }
}
