using System;
using Akka.Actor;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy.Akka
{
    public static class DiActorContextAdapterExtensions
    {
        public static Props<T> Props<T, TActor>(this DIActorContextAdapter adapter) where TActor : ActorBase, IReceive<T>
            =>  new Props<T>(adapter.Props<TActor>());

        public static Props<T> Props<T>(this DIActorContextAdapter adapter, Type actorType) 
            => new Props<T>(adapter.Props(actorType));
    }
}