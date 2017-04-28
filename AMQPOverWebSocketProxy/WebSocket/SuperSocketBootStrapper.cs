using System;
using System.Linq;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Metadata;
using SuperSocket.SocketEngine;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class SuperSocketBootStrapper : DefaultBootstrap
    {
        private readonly IServiceProvider _serviceProvider;

        public SuperSocketBootStrapper(IServiceProvider serviceProvider, ISuperSocketConfigurationProvider configurationProvider) : base(GetConfigurationSource(configurationProvider))
        {
            _serviceProvider = serviceProvider;
        }

        private static IConfigurationSource GetConfigurationSource(ISuperSocketConfigurationProvider configurationProvider)
        {
            return configurationProvider.Get();
        }

        protected override IWorkItem CreateWorkItemInstance(string serviceTypeName, StatusInfoAttribute[] serverStatusMetadata)
        {
            var serverType = Type.GetType(serviceTypeName);
            if (serverType == null)
            {
                throw new InvalidOperationException($"Could not find service type {serviceTypeName}. Are you sure the name is correct?");
            }

            var server = _serviceProvider.GetService(serverType);
            if (server == null)
            {
                throw new InvalidOperationException($"Could not resolve service {serverType.FullName}. Is it correctly registered in the service locator {_serviceProvider.GetType().FullName}?");
            }

            if (server.GetType().GetInterfaces().Contains(typeof(IWorkItem)))
            {
                return (IWorkItem)server;
            }

            throw new InvalidOperationException($"{serverType.FullName} must implement {typeof(IWorkItem)}.");
        }
    }
}