using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.Akka;

namespace AMQPOverWebSocketProxy.Actors
{
    public class ActorProxyActor<TActor, TMessage> : UntypedActor<TMessage>
        where TActor : ActorBase, IReceive<TMessage>
    {
        private readonly IActorResolver _resolver;

        public ActorProxyActor(IActorResolver resolver)
        {
            _resolver = resolver;
        }

        protected override void OnReceive(TMessage message)
        {
            var actor = _resolver.Resolve<TActor, TMessage>(actorType => Context.ActorOf(Context.DI().Props<TMessage>(actorType)));
            actor.Tell(message);
        }
    }
}