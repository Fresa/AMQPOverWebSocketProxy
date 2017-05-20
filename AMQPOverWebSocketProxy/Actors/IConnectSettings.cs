using System.Collections.Generic;

namespace AMQPOverWebSocketProxy.Actors
{
    public interface IConnectSettings
    {
        string UserName { get; }
        string Password { get; }
        string VirtualHost { get; }
        IEnumerable<string> HostNames { get; }
        int Port { get; }
    }
}