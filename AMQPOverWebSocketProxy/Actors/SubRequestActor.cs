using System;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.Akka;
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

        public class SubRequestParsed
        {
            public TRequest Request { get; }

            public SubRequestParsed(TRequest request)
            {
                Request = request;
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
            
            var messageProxyActor = Context.ActorOf(
                Context.DI().Props<
                    SubRequestParsed, 
                    MessageProxyActor<
                        UntypedActor<SubRequestParsed>, 
                        SubRequestParsed>>());

            messageProxyActor.Tell(new SubRequestParsed(amqpRequest));
        }
    }
}