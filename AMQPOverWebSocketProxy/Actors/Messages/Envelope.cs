using AMQPOverWebSocketProxy.Akka;

namespace AMQPOverWebSocketProxy.Actors.Messages
{
    public class Envelope
    {
        public class Adress
        {
            public Adress(string virtualHost, string exchange)
                : this(virtualHost, exchange, "")
            {
            }

            public Adress(string virtualHost, string exchange, string routingKey)
            {
                VirtualHost = virtualHost;
                Exchange = exchange;
                RoutingKey = routingKey ?? "";
            }

            public string VirtualHost { get; }
            public string Exchange { get; }
            public string RoutingKey { get; }
        }

        public Envelope(object message, Adress to, IActorRef<MessageSent> messageSentReceiver)
        {
            Message = message;
            To = to;
            MessageSentReceiver = messageSentReceiver;
        }

        public object Message { get; }
        public Adress To { get; }
        public IActorRef<MessageSent> MessageSentReceiver { get; }
    }
}