using Akka.Actor;
using AMQPOverWebSocketProxy.Actors;
using AMQPOverWebSocketProxy.Akka;
using Common.Serialization;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public abstract class BaseCommand<TRequest> : ISubCommand<WebSocketSession>
    {
        private readonly ISerializer _serializer;
        private readonly IActorRefFactory _actorRefFactory;

        protected BaseCommand(ISerializer serializer, IActorRefFactory actorRefFactory)
        {
            _serializer = serializer;
            _actorRefFactory = actorRefFactory;
        }

        public void ExecuteCommand(WebSocketSession session, SubRequestInfo requestInfo)
        {
            var sessionSenderActor = _actorRefFactory.ActorOf(Props.Create(() => new WebSocketMessageSenderActor(session)));
            var subRequestActor = _actorRefFactory.ActorOf(
                Props<TRequest>.Create(() => new SubRequestActor<TRequest>(_serializer, sessionSenderActor)));

            subRequestActor.Tell(new SubRequestActor<TRequest>.SubRequestReceived(requestInfo));
        }

        public abstract string Name { get; }
    }
}