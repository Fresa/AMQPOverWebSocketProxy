using System;
using System.Collections.Generic;
using SuperSocket.SocketBase.Config;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class RandomPortConfigProvider : ISuperSocketConfigurationProvider
    {
        public IConfigurationSource Get()
        {
            var port = new Random().Next(2000, 3000);
            return new ConfigurationSource
            {
                Servers = new List<IServerConfig>
                {
                    new ServerConfig
                    {
                        Ip = "Any",
                        Port = port,
                        ServerTypeName = "AMQPOverWebSocketProxyServer",
                        Name = "AMQPOverWebSocketProxyServer" + port
                    }
                },
                ServerTypes = new List<ITypeProvider>
                {
                    new TypeProviderConfig
                    {
                        Name = "AMQPOverWebSocketProxyServer",
                        Type = "AMQPOverWebSocketProxy.WebSocket.WebSocketServer, AMQPOverWebSocketProxy"
                    }
                }
            };
        }
    }
}