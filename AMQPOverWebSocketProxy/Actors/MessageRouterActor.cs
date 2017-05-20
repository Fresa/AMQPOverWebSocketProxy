using Akka.Actor;
using AMQPOverWebSocketProxy.Akka;

namespace AMQPOverWebSocketProxy.Actors
{
    public class MessageRouterActor<TSend> : ReceiveActor<IActorRef<TSend>>
    {
        private readonly TSend _message;
        private readonly IActorRef _from;

        public MessageRouterActor(TSend message, IActorRef from)
        {
            _message = message;
            _from = @from;
        }

        protected override void OnReceive(IActorRef<TSend> actor)
        {
            actor.Tell(_message, _from);
        }
    }
}