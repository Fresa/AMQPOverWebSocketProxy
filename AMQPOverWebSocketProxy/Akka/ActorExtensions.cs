using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public static class ActorExtensions
    {
        public static TypedRef<T> ActorOf<T>(this IActorRefFactory fac, Props<T> props) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying));

        public static TypedRef<T> ActorOf<T>(this IActorRefFactory fac, Props<T> props, string name) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying, name));

        public static TypedRef<T> Watch<T>(this IActorContext ctx, TypedRef<T> tref)
        {
            ctx.Watch(tref.Ref);
            return tref;
        }

        public static TypedRef<T> Unwatch<T>(this IActorContext ctx, TypedRef<T> tref)
        {
            ctx.Unwatch(tref.Ref);
            return tref;
        }
    }
}