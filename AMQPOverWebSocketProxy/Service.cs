using Akka.Actor;
using Akka.Configuration;
using AMQPOverWebSocketProxy.Actors;
using AMQPOverWebSocketProxy.IOC;
using Topshelf;

namespace AMQPOverWebSocketProxy
{
    //public sealed class Service : IService
    //{
    //    private ActorSystem _actorSystem;

    //    public bool Start(HostControl hostControl, IDependencyResolverFactory dependencyResolverFactory)
    //    {
    //        _actorSystem = ActorSystem.Create("AMQP-over-WebSocket-Proxy-Actors");
    //        dependencyResolverFactory.Create(_actorSystem);

    //        _actorSystem.ActorOf(Props.Create(() => new SocketSupervisor(hostControl.Stop)));
    //        return true;
    //    }

    //    public async void Stop()
    //    {
    //        await _actorSystem.Terminate();
    //    }
    //}

    public sealed class RemoteService : IService
    {
        private ActorSystem _actorSystem;

        public bool Start(HostControl hostControl, IDependencyResolverFactory dependencyResolverFactory)
        {
            _actorSystem = ActorSystem.Create("AMQP-over-WebSocket-Proxy-Actors", ConfigurationFactory.ParseString(@"
            akka {  
                actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                remote {
                    helios.tcp {
                        port = 8090
                        hostname = localhost
                    }
                }
            }"));
            dependencyResolverFactory.Create(_actorSystem);

            //_actorSystem.ActorOf(Props.Create(() => new SocketSupervisor(hostControl.Stop)));
            return true;
        }

        public async void Stop()
        {
            await _actorSystem.Terminate();
        }
    }
}