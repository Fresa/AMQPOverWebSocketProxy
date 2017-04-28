using AMQPOverWebSocketProxy.IOC;
using Topshelf;

namespace AMQPOverWebSocketProxy
{
    public interface IService
    {
        bool Start(HostControl hostControl, IDependencyResolverFactory dependencyResolverFactory);
        void Stop();
    }
}