using Akka.Actor;
using AMQPOverWebSocketProxy.IOC;
using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;

namespace AMQPOverWebSocketProxy
{
    internal class Program
    {
        private static readonly Container Container = new Container();
        private static ActorSystem _actorSystem;

        private static void Main()
        {
            Configure();

            HostFactory.Run(config =>
            {
                config.UseSimpleInjector(Container);
                config.UseNLog();

                config.Service<IService>(configurator =>
                {
                    configurator.ConstructUsingSimpleInjector();
                    configurator.WhenStarted((service, control) => service.Start(control));
                    configurator.WhenStopped(async service =>
                    {
                        service.Stop();
                        await _actorSystem.Terminate();
                    });
                });

                config.SetDescription("A proxy for the AMQP protocol over WebSocket");
                config.SetDisplayName("AMQP over WebSocket Proxy");
                config.SetServiceName("AMQP over WebSocket Proxy");
            });
        }

        private static void Configure()
        {
            _actorSystem = ActorSystem.Create("AMQP-over-WebSocket-Proxy-Actors");

            Container.RegisterSingleton<IService, Service>();

            Configurer.Configure(Container, _actorSystem);
        }
    }
}
