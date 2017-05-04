namespace AMQPOverWebSocketProxy.Actors
{
    internal class SubRequestParsed<T>
    {
        public T AmqpRequest { get; }

        public SubRequestParsed(T amqpRequest)
        {
            AmqpRequest = amqpRequest;
        }
    }
}