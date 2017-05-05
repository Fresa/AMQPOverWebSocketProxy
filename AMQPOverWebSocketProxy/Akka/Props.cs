using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Util;

namespace AMQPOverWebSocketProxy.Akka
{
    public struct Props<T> : ISurrogated
    {
        public readonly Props Underlying;

        public Props(Props props) : this()
        {
            Underlying = props ?? throw new ArgumentNullException(nameof(props), $"{this} has received null instead of {nameof(Props)}");
        }

        public static Props<T> Create<TActor>(Expression<Func<TActor>> fac) where TActor : UntypedActor<T>
            => new Props<T>(Props.Create(fac));


        public ISurrogate ToSurrogate(ActorSystem system) => new TypedPropsSurrogate<T>(Underlying.ToSurrogate(system));
    }
}