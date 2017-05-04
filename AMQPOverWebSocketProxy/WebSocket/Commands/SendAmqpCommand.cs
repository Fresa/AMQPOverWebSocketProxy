namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public class SendAmqpCommand : Command<AmqpRequest<object>>
    {
        public SendAmqpCommand(IActorRefGeneric<AmqpRequest<object>> amqpRequestActor) : base(amqpRequestActor)
        {
        }

        public override string Name { get; } = "Send";
    }
}