using System;
using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.WebSocket.Commands;
using AMQPOverWebSocketProxy.WebSocket.Messages;
using Common.Serialization;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.Actors
{
    public class SubRequestActor<TRequest> : ReceiveActor
    {
        #region Messages

        public class SubRequestReceived
        {
            public SubRequestInfo RequestInfo { get; }

            public SubRequestReceived(SubRequestInfo requestInfo)
            {
                RequestInfo = requestInfo;
            }
        }

        #endregion

        private readonly IActorRef _webSocketMessageSenderActor;
        private readonly ISerializer _serializer;

        public SubRequestActor(ISerializer serializer, IActorRef webSocketMessageSenderActor)
        {
            _webSocketMessageSenderActor = webSocketMessageSenderActor;
            _serializer = serializer;
            Receive<SubRequestReceived>(request => Handler(request));
        }

        private void Handler(SubRequestReceived subRequestReceived)
        {
            TRequest amqpRequest;
            try
            {
                amqpRequest = _serializer.DeserializeFromString<TRequest>(subRequestReceived.RequestInfo.Body);
            }
            catch (Exception ex)
            {
                _webSocketMessageSenderActor.Tell(new WebSocketMessageSenderActor.SendMessage(new FormatErrorMessage($"Incorrectly formatted message. {ex.Message}")));
                return;
            }

            var sendCommandActor = Context.ActorOf(Context.DI().Props<AmqpSendCommandActor<TRequest>>());
            sendCommandActor.Tell(new AmqpSendCommandActor2.SubRequestParsed<TRequest>(amqpRequest));
        }
    }

    public class AmqpSendCommandActor2 : AmqpSendCommandActor<AmqpRequest<object>>
    {
        #region Messages
        internal class SubRequestParsed<T>
        {
            public T AmqpRequest { get; }

            public SubRequestParsed(T amqpRequest)
            {
                AmqpRequest = amqpRequest;
            }
        }
        #endregion

        public AmqpSendCommandActor2()
        {
            Receive<SubRequestParsed<AmqpRequest<object>>>(parsed =>
            {

            });
        }
    }

    public abstract class AmqpSendCommandActor<TRequest> : ReceiveActor
    {
        
    }
}