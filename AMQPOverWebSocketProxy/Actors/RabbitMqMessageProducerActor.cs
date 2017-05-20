using System;
using AMQPOverWebSocketProxy.Actors.Messages;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.Messaging;
using Common.Serialization;
using RabbitMQ.Client;

namespace AMQPOverWebSocketProxy.Actors
{
    public class RabbitMqMessageProducerActor : ReceiveActor<Envelope>
    {
        private readonly IModel _model;
        private readonly ISerializer _serializer;

        public RabbitMqMessageProducerActor(IModel model, ISerializer serializer)
        {
            _model = model;
            _serializer = serializer;
        }

        protected override void OnReceive(Envelope envelope)
        {
            var properties = new RabbitMQMessageProperties();
            var messageBytes = _serializer.Serialize(envelope.Message, properties.ContentEncoding);

            var rabbitMqProperties = _model.CreateBasicProperties();
            rabbitMqProperties.ContentType = _serializer.ContentType;
            rabbitMqProperties.ContentEncoding = properties.ContentEncoding.BodyName;
            rabbitMqProperties.CorrelationId = properties.CorrelationId;
            rabbitMqProperties.Expiration = properties.Expiration;
            rabbitMqProperties.Headers = properties.Headers;
            rabbitMqProperties.MessageId = properties.MessageId;
            rabbitMqProperties.Persistent = properties.DeliveryMode == DeliveryMode.Persistent;
            rabbitMqProperties.ReplyTo = properties.ReplyTo;
            rabbitMqProperties.Priority = properties.Priority;
            rabbitMqProperties.Timestamp = new AmqpTimestamp((long)properties.Timestamp.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            rabbitMqProperties.UserId = properties.UserId;

            var adress = envelope.To;
            _model.BasicPublish(adress.Exchange, adress.RoutingKey, rabbitMqProperties, messageBytes);

            envelope.MessageSentReceiver.Tell(new MessageSent(properties.MessageId, properties.CorrelationId));
        }

        protected override void PostStop()
        {
            _model.Close();
            _model.Dispose();
        }
    }
}