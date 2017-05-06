using System;
using Akka.Actor;
using AMQPOverWebSocketProxy.WebSocket;

namespace AMQPOverWebSocketProxy.Actors
{
    public class WebSocketMessageSenderActor : ReceiveActor
    {
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
            Receive<SendMessage>(message =>
            {
                if (session.Connected)
                {
                    session.Send(session.AppServer.JsonSerialize(message));
                }

                throw new InvalidOperationException("Websocket session is closed.");
            });
        }
    }
}