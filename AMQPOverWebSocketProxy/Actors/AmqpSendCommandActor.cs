using System;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.WebSocket.Commands;

namespace AMQPOverWebSocketProxy.Actors
{
    public class AmqpSendCommandActor : UntypedActor<AmqpSendCommandActor.SubRequestParsed<AmqpRequest<object>>>
    {
        #region Messages
        public class SubRequestParsed<TRequest>
        {
            public TRequest AmqpRequest { get; }

            public SubRequestParsed(TRequest amqpRequest)
            {
                AmqpRequest = amqpRequest;
            }
        }
        #endregion

        protected override void OnReceive(SubRequestParsed<AmqpRequest<object>> message)
        {
            throw new NotImplementedException();
        }
    }
}