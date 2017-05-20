namespace AMQPOverWebSocketProxy.Actors.Messages
{
    public class MessageSent
    {
        public MessageSent(string messageId, string correlationId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
        }

        public string MessageId { get; }
        public string CorrelationId { get; }
    }
}