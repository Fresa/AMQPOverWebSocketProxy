using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.WebSocket.Commands;
using Common.Serialization;
using RabbitMQ.Client;

namespace AMQPOverWebSocketProxy.Actors
{
    public class AmqpReceiveCommandActor : UntypedActor<SubRequestActor<AmqpRequest<object>>.SubRequestParsed>
    {
        private readonly IActorRef _connectionPoolActor;

        public AmqpReceiveCommandActor(IActorRef connectionPoolActor)
        {
            _connectionPoolActor = connectionPoolActor;
        }   

        protected override void OnReceive(SubRequestActor<AmqpRequest<object>>.SubRequestParsed message)
        {
            var coordinatingActor = Context.ActorOf(Props.Create(() => new AmqpSendCommandCoordinatingActor(_connectionPoolActor)));
            coordinatingActor.Tell(new Send<AmqpRequest<object>>(message.Request));
        }
    }

    public class AmqpSendCommandCoordinatingActor : ReceiveActor
    {
        //todo: Handle restart without loosing messages!
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<AmqpRequest<object>>> _pendingMessages = new ConcurrentDictionary<Guid, ConcurrentQueue<AmqpRequest<object>>>();

        public AmqpSendCommandCoordinatingActor(IActorRef connectionPoolActor)
        {
            Receive<Send<AmqpRequest<object>>>(send =>
            {
                var connection = send.Message.Connection;
                var createMessageProducer = new CreateMessageProducer(connection.UserName, connection.Password,
                    connection.VirtualHost, Self);

                var messages = _pendingMessages.GetOrAdd(createMessageProducer.Id, new ConcurrentQueue<AmqpRequest<object>>(new[] { send.Message }));
                messages.Enqueue(send.Message);

                connectionPoolActor.Tell(createMessageProducer);
            });

            Receive<MessageProducerAvailable>(messageProducerAvailableMessage =>
            {
                if (_pendingMessages.TryRemove(messageProducerAvailableMessage.MessageId, out var messages))
                {
                    foreach (var amqpMessage in messages)
                    {
                        messageProducerAvailableMessage.Producer.Tell(new Send<AmqpRequest<object>>(amqpMessage));
                    }
                }
            });
        }
    }

    
    internal class MessageProducerAvailable
    {
        public MessageProducerAvailable(IActorRef producer, Guid messageId)
        {
            Producer = producer;
            MessageId = messageId;
        }

        public IActorRef Producer { get; }
        public Guid MessageId { get; }
    }

    public class Send<TMessage>
    {
        public Send(TMessage message)
        {
            Message = message;
        }

        public TMessage Message { get; }
    }

    public class PendingPublish<TMessage>
    {
        public PendingPublish(Guid id, TMessage message)
        {
            Id = id;
            Message = message;
        }

        public Guid Id { get; }
        public TMessage Message { get; }
    }

    public class ConnectionPoolActor : ReceiveActor
    {
        private readonly Dictionary<Guid, IActorRef> _messageProducers = new Dictionary<Guid, IActorRef>();

        public ConnectionPoolActor()
        {
            Receive<CreateMessageProducer>(createProducer =>
            {
                if (_messageProducers.TryGetValue(createProducer.Id, out var messageProducer) == false)
                {
                    messageProducer = Context.ActorOf(Props.Create(() => new MessageProducerActor()));
                    _messageProducers.Add(createProducer.Id, messageProducer);
                }
                createProducer.Sender.Tell(new MessageProducerAvailable(messageProducer, createProducer.Id));
            });
        }
    }

    public class RabbitMqMessageProducerActor : ReceiveActor
    {
        private readonly IModel _model;
        private readonly ISerializer _serializer;

        public RabbitMqMessageProducerActor(IModel model, ISerializer serializer)
        {
            _model = model;
            _serializer = serializer;
        }

        public RabbitMqMessageProducerActor()
        {
            Receive<AmqpMessage>(message =>
            {
                var properties = message.Properties;
                var messageBytes = _serializer.Serialize(message.Message, properties.ContentEncoding);

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

                _model.BasicPublish(message.Exchange, message.RoutingKey, rabbitMqProperties, messageBytes);
            });
        }

        protected override void PostStop()
        {
            _model.Close();
            _model.Dispose();
        }
    }


    public class ConnectionFactoryActor : ReceiveActor
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IEnumerable<string> _hostNames;
        private readonly int _port;

        public ConnectionFactoryActor(IConnectionFactory connectionFactory, IEnumerable<string> hostNames, int port)
        {
            _connectionFactory = connectionFactory;
            _hostNames = hostNames;
            _port = port;

            Receive<CreateMessageProducer>(connect =>
            {
                _connectionFactory.Connect() new ConnectionSettings(connect.UserName, connect.Password, connect.VirtualHost,
                    _port, _hostNames)

                connect.Sender.Tell();
            });
        }
    }

    public class CreateMessageProducer
    {
        public CreateMessageProducer(string userName, string password, string virtualHost, IActorRef sender)
        {
            UserName = userName;
            Password = password;
            VirtualHost = virtualHost;
            Sender = sender;

            Id = new Guid($"{userName}-{password}-{virtualHost}");
        }

        public string UserName { get; }
        public string Password { get; }
        public string VirtualHost { get; }
        public IActorRef Sender { get; }

        public Guid Id { get; }
    }

    public interface IConnectSettings
    {
        string UserName { get; }
        string Password { get; }
        string VirtualHost { get; }
        IEnumerable<string> HostNames { get; }
        int Port { get; }
    }

    public class ConnectionSettings : IConnectSettings
    {
        public ConnectionSettings(string userName, string password, string virtualHost, int port, IEnumerable<string> hostNames)
        {
            UserName = userName;
            Password = password;
            VirtualHost = virtualHost;
            Port = port;
            HostNames = hostNames;
        }

        public string UserName { get; }
        public string Password { get; }
        public string VirtualHost { get; }
        public IEnumerable<string> HostNames { get; }
        public int Port { get; }
    }

    public interface IConnectionFactory
    {
        IConnection Connect(IConnectSettings settings);
    }

    public class RabbitMQConnectionFactory : IConnectionFactory
    {
        private readonly ISerializer _serializer;

        public RabbitMQConnectionFactory(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public IConnection Connect(IConnectSettings settings)
        {
            var factory = new ConnectionFactory
            {
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost
            };

            return new RabbitMQConnection(factory.CreateConnection(settings.HostNames.ToList()), _serializer);
        }
    }

    public enum DeliveryMode
    {
        NonPersistent,
        Persistent
    }

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

    public interface IConnection : IDisposable
    {
        IMessageProducer Create();
    }

    public class RabbitMQMessageProducer : IMessageProducer
    {
        private readonly IModel _model;
        private readonly ISerializer _serializer;
        private bool _isDisposing;
        private bool _isDisposed;

        public RabbitMQMessageProducer(IModel model, ISerializer serializer)
        {
            _model = model;
            _serializer = serializer;
        }

        public void Publish<T>(string exchange, string routingKey, T message, IMessageProperties properties)
        {
            var messageBytes = _serializer.Serialize(message, properties.ContentEncoding);

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

            _model.BasicPublish(exchange, routingKey, rabbitMqProperties, messageBytes);
        }

        public void Dispose()
        {
            if (_isDisposed || _isDisposing)
            {
                return;
            }

            _isDisposing = true;

            _model.Close();
            _model.Dispose();

            _isDisposing = false;
            _isDisposed = true;
        }
    }

    public interface IMessageProducer : IDisposable
    {
    }

    public class RabbitMQConnection : IConnection
    {
        private readonly RabbitMQ.Client.IConnection _connection;
        private readonly ISerializer _serializer;
        private bool _isDisposed;
        private bool _isDisposing;

        public RabbitMQConnection(RabbitMQ.Client.IConnection connection, ISerializer serializer)
        {
            _connection = connection;
            _serializer = serializer;
        }

        public void Dispose()
        {
            if (_isDisposed || _isDisposing)
            {
                return;
            }

            _isDisposing = true;

            _connection.Close();
            _connection.Dispose();

            _isDisposing = false;
            _isDisposed = true;
        }

        public IMessageProducer Create()
        {
            if (_isDisposed || _isDisposing)
            {
                throw new InvalidOperationException("Connection has been disposed.");
            }
            if (_connection.IsOpen == false)
            {
                throw new InvalidOperationException("Cannot create message producer, the connection is closed.");
            }

            return new RabbitMQMessageProducer(_connection.CreateModel(), _serializer);
        }
    }
}