using System;
using System.Collections.Generic;
using AMQPOverWebSocketProxy.WebSocket.Messages;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public abstract class BaseCommand<TMessage> : ISubCommand<WebSocketSession>
    {
        public void ExecuteCommand(WebSocketSession session, SubRequestInfo requestInfo)
        {
            Request<TMessage> request;
            try
            {
                request =
                    (Request<TMessage>) session.AppServer.JsonDeserialize(requestInfo.Body, typeof(Request<TMessage>));
            }
            catch (Exception ex)
            {
                session.Send(session.AppServer.JsonSerialize(new FormatErrorMessage($"Incorrectly formatted message. {ex.Message}")));
                return;
            }
            Execute(session, request.Body, request.Headers);
        }
        
        protected abstract void Execute(WebSocketSession session, TMessage message, IDictionary<string, object> headers);

        public abstract string Name { get; }
    }
}