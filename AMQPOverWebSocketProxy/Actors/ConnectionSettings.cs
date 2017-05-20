using System.Collections.Generic;

namespace AMQPOverWebSocketProxy.Actors
{
    public class ConnectionSettings : IConnectSettings
    {
        public ConnectionSettings(string userName, string password, string virtualHost, int port, IEnumerable<string> hostNames)
        {
            UserName = userName;
            Password = password;
            VirtualHost = virtualHost;
            Port = port;
            HostNames = hostNames;
        }

        public string UserName { get; }
        public string Password { get; }
        public string VirtualHost { get; }
        public IEnumerable<string> HostNames { get; }
        public int Port { get; }
    }
}