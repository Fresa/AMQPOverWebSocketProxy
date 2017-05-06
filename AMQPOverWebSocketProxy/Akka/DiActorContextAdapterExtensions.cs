using System;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy.Akka
{
    public static class DiActorContextAdapterExtensions
    {
        public static Props<T> Props<T, TActor>(this DIActorContextAdapter adapter) where TActor : UntypedActor<T>
            =>  new Props<T>(adapter.Props<TActor>());

        public static Props<T> Props<T>(this DIActorContextAdapter adapter, Type actorType) 
            => new Props<T>(adapter.Props(actorType));
    }
}