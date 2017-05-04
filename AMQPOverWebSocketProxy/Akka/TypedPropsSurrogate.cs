using Akka.Actor;
using Akka.Util;

namespace AMQPOverWebSocketProxy.Akka
{
    internal struct TypedPropsSurrogate<T> : ISurrogate
    {
        public readonly ISurrogate PropsSurrogate;

        public TypedPropsSurrogate(ISurrogate propsSurrogate) : this()
        {
            PropsSurrogate = propsSurrogate;
        }

        public ISurrogated FromSurrogate(ActorSystem system) => new Props<T>((Props)PropsSurrogate.FromSurrogate(system));
    }
}