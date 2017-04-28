using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using SimpleInjector;

namespace AMQPOverWebSocketProxy.IOC
{
    public class SimpleInjectorDependencyResolverFactory : IDependencyResolverFactory
    {
        private readonly Container _container;

        public SimpleInjectorDependencyResolverFactory(Container container)
        {
            _container = container;
        }

        public IDependencyResolver Create(ActorSystem actorSystem)
        {
            return new SimpleInjectorDependencyResolver(_container, actorSystem);
        }
    }
}