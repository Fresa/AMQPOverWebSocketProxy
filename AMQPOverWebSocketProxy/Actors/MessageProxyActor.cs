using Akka.DI.Core;
using AMQPOverWebSocketProxy.Akka;

namespace AMQPOverWebSocketProxy.Actors
{
    public class MessageProxyActor<TActor, TMessage> : UntypedActor<TMessage>
        where TActor : UntypedActor<TMessage>
    {
        private readonly IActorResolver _resolver;

        public MessageProxyActor(IActorResolver resolver)
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