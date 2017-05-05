using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public static class ActorExtensions
    {
        public static IActorRef<T> ActorOf<T>(this IActorRefFactory fac, Props<T> props) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying));

        public static IActorRef<T> ActorOf<T>(this IActorRefFactory fac, Props<T> props, string name) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying, name));

        public static IActorRef<T> Watch<T>(this IActorContext ctx, TypedRef<T> tref)
        {
            ctx.Watch(tref.Ref);
            return tref;
        }

        public static IActorRef<T> Unwatch<T>(this IActorContext ctx, TypedRef<T> tref)
        {
            ctx.Unwatch(tref.Ref);
            return tref;
        }

        public static IActorRef<T> ActorOf<T>(this IActorContext fac, Props<T> props) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying));

        public static IActorRef<T> ActorOf<T>(this IActorContext fac, Props<T> props, string name) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying, name));
    }
}