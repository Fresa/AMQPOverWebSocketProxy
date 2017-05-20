using System;
using System.Collections.Generic;
using System.Text;

namespace AMQPOverWebSocketProxy.Messaging
{
    public interface IMessageProperties
    {
        Encoding ContentEncoding { get; }

        string CorrelationId { get; }

        DeliveryMode DeliveryMode { get; }

        string Expiration { get; }

        IDictionary<string, object> Headers { get; }

        string MessageId { get; }

        byte Priority { get; }

        string ReplyTo { get; }

        DateTime Timestamp { get; }

        string UserId { get; }
    }
}