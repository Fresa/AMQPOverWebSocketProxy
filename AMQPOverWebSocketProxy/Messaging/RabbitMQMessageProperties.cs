using System;
using System.Collections.Generic;
using System.Text;

namespace AMQPOverWebSocketProxy.Messaging
{
    public class RabbitMQMessageProperties : IMessageProperties
    {
        public Encoding ContentEncoding { get; } = Encoding.UTF8;
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.NonPersistent;
        public string Expiration { get; set; }
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        private byte _priority;
        public byte Priority
        {
            get => _priority;
            set
            {
                if (value > 9)
                {
                    throw new ArgumentException($"{nameof(Priority)} must be between 0-9.");
                }

                _priority = value;
            }
        }

        public string ReplyTo { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = "Unknown";
    }
}