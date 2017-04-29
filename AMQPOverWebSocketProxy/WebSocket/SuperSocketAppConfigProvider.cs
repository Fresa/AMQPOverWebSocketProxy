using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine.Configuration;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class SuperSocketAppConfigProvider : ISuperSocketConfigurationProvider
    {
        public IConfigurationSource Get()
        {
            var obj = ConfigurationManager.GetSection("superSocket") ?? ConfigurationManager.GetSection("socketServer");
            if (obj == null)
            {
                throw new ConfigurationErrorsException("Missing 'superSocket' or 'socketServer' configuration section.");
            }
            var config = obj as IConfigurationSource;
            if (config == null)
            {
                throw new ConfigurationErrorsException("Invalid 'superSocket' or 'socketServer' configuration section.");
            }

            return config;
        }
    }

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