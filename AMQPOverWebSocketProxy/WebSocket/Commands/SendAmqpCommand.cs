using System.Text;
using Common.Messaging;
using Common.Messaging.Producers;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public class SendAmqpCommand : BaseAmqpCommand<object>
    {
        private IMessageProducer _messageProducer;

        public SendAmqpCommand(IMessageProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        protected override void Execute(WebSocketSession session, AmqpRequest<object> request)
        {
            _messageProducer.Produce(request.Exchange, new Message
            {
                Body = Encoding.UTF8.GetBytes(session.AppServer.JsonSerialize(request.Body)),
                Headers = request.Headers,
                CorrelationId = request.CorrelationId,
                MessageId = request.MessageId,
                MessageKey = request.MessageKey
            });
        }

        public override string Name { get; } = "Send";
    }

    public class SendAmqpCommand2 : Command<AmqpRequest<object>>
    {
        public SendAmqpCommand2(IActorRefGeneric<AmqpRequest<object>> objectAmqpRequestActor) : base(objectAmqpRequestActor)
        {
        }

        public override string Name { get; } = "Send";
    }

    public class ConfirmCommandProcessed
    {
    }

    public class AmqpCommandRequestReceived<T>
    {
        public AmqpCommandRequestReceived(AmqpRequest<T> request, WebSocketSession session)
        {
            throw new System.NotImplementedException();
        }
    }
    
}