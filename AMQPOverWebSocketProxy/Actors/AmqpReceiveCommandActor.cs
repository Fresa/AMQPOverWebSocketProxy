using Akka.Actor;
using AMQPOverWebSocketProxy.Actors.Messages;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.WebSocket.Commands;
using Envelope = AMQPOverWebSocketProxy.Actors.Messages.Envelope;

namespace AMQPOverWebSocketProxy.Actors
{
    public class AmqpReceiveCommandActor : ReceiveActor<SubRequestActor<AmqpRequest<object>>.SubRequestParsed>
    {
        private readonly IActorRef _connectionFactoryActor;

        public AmqpReceiveCommandActor(IActorRef connectionFactoryActor)
        {
            _connectionFactoryActor = connectionFactoryActor;
        }

        protected override void OnReceive(SubRequestActor<AmqpRequest<object>>.SubRequestParsed message)
        {
            var messageProxyActor = Context.ActorOf(Props<MessageSent>.Create(() =>
               new MessageProxyActor<MessageSent, WebSocketMessageSenderActor.SendMessage>(message.MessageSender, sent =>
                   new WebSocketMessageSenderActor.SendMessage(sent), Self)));

            var messageRouterActor = Context.ActorOf(Props<IActorRef<Envelope>>.Create(() =>
               new MessageRouterActor<Envelope>(
                   new Envelope(message.Request.Body,
                       new Envelope.Adress(message.Request.VirtualHost, message.Request.Exchange, message.Request.MessageKey),
                       messageProxyActor), Self)));

            _connectionFactoryActor.Tell(new ConnectionFactoryActor.Connect(new ConnectionFactoryActor.VirtualConnectionSettings(message.Request.Connection.UserName,
                message.Request.Connection.Password, message.Request.VirtualHost), messageRouterActor), Self);            
        }
    }
}