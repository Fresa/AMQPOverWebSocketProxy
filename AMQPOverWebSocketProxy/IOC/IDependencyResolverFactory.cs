using Akka.Actor;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy.IOC
{
    public interface IDependencyResolverFactory
    {
        IDependencyResolver Create(ActorSystem actorSystem);
    }
}