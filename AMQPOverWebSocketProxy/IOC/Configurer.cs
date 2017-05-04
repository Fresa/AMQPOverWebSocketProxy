﻿using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using AMQPOverWebSocketProxy.Logging;
using AMQPOverWebSocketProxy.Serialization;
using AMQPOverWebSocketProxy.WebSocket;
using AMQPOverWebSocketProxy.WebSocket.Commands;
using Common.Serialization;
using Common.Serialization.Serializer;
using Newtonsoft.Json;
using SimpleInjector;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Sockets;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.IOC
{
    public class Configurer
    {
        public static void Configure(Container container, ActorSystem actorSystem)
        {
            /* WebSocket */
            container.RegisterSingleton<IBootstrap, SuperSocketBootStrapper>();
            container.RegisterSingleton<ISuperSocketConfigurationProvider, RandomPortConfigProvider>();
            container.RegisterSingleton<ILogFactory, DefaultLogFactory>();
            container.RegisterSingleton<ISubProtocol<WebSocketSession>, CommandProtocol>();
            container.RegisterSingleton<ISocketFactory, PassthroughSocketFactory>();
            container.RegisterSingleton<IRequestInfoParser<SubRequestInfo>, JsonRequestInfoParser>();

            //container.RegisterSingleton<IActorRefGeneric<AmqpRequest<object>>>(() => actorSystem.ActorOf(container.GetInstance<IDependencyResolver>().Create<AmqpRequest<object>, SubRequestActor<AmqpRequest<object>>>(), "sub-request-actor-for-object-based-amqp-requests"));
            container.RegisterCollection<ISubCommand<WebSocketSession>>(new[]
            {
                typeof(SendAmqpCommand2)
            });

            container.RegisterSingleton<ISerializer>(() =>
                new JsonNetSerializer(JsonSerializer.Create(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    ContractResolver = new NonPublicMembersContractResolver
                    {
                        CamelCase = true
                    }
                })));

            container.RegisterSingleton<IRequestActorFactory, RequestActorFactory>();

            container.RegisterSingleton(() => actorSystem);
            container.RegisterSingleton<IDependencyResolver>(() => new SimpleInjectorDependencyResolver(container, actorSystem));

            var containerRegistration = Lifestyle.Singleton.CreateRegistration(() => container, container);
            container.AddRegistration<IServiceProvider>(containerRegistration);
            container.AddRegistration<Container>(containerRegistration);

            container.Verify();
        }
    }


}