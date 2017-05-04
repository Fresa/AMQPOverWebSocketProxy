using System;
using Akka.Actor;
using AMQPOverWebSocketProxy.Actors;
using AMQPOverWebSocketProxy.IOC;
using SimpleInjector;

namespace AMQPOverWebSocketProxyDeployer
{
    class Program
    {
        private static readonly Container Container = new Container();

        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("AMQP-over-WebSocket-Proxy-Actors-Deployer"))
            {
                Configurer.Configure(Container, system);
                
                system.ActorOf(Props.Create(() => new SocketSupervisorActor()), "socket-supervisor-actor");

                Console.ReadKey();
            }
        }
    }
}
