using System;
using Akka.Actor;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.WebSocket;

namespace AMQPOverWebSocketProxy.Actors
{
    public class WebSocketMessageSenderActor : ReceiveActor<WebSocketMessageSenderActor.SendMessage>
    {
        private readonly WebSocketSession _session;

        #region Messages

        public class SendMessage
        {
            public SendMessage(object message)
            {
                Message = message;
            }

            public object Message { get; }
        }

        #endregion

        public WebSocketMessageSenderActor(WebSocketSession session)
        {
            _session = session;
        }

        protected override void OnReceive(SendMessage message)
        {
            if (_session.Connected)
            {
                _session.Send(_session.AppServer.JsonSerialize(message));
            }

            throw new InvalidOperationException("Websocket session is closed.");
        }
    }
}