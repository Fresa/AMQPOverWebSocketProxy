using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.WebSocket.Commands;

namespace AMQPOverWebSocketProxy.IOC
{
    public static class SimpleInjectorDependencyResolverExtensions
    {
        public static Props<T> Create<T, TActor>(this IDependencyResolver resolver) 
            where TActor : ActorBase
        {
            return new Props<T>(resolver.Create<TActor>());
        }
    }
}