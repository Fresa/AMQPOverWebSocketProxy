using Akka.Actor;
using Akka.Util;

namespace AMQPOverWebSocketProxy.Akka
{
    internal struct TypeRefSurrogate<T> : ISurrogate
    {
        public readonly ISurrogate RefSurrogate;

        public TypeRefSurrogate(ISurrogate refSurrogate) : this()
        {
            RefSurrogate = refSurrogate;
        }

        public ISurrogated FromSurrogate(ActorSystem system) => new TypedActorRef<T>((IActorRef)RefSurrogate.FromSurrogate(system));
    }
}