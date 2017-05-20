
using Akka.Actor;

namespace AMQPOverWebSocketProxy.Actors.Messages
{
    public class Send<TMessage> : Send
    {
        public Send(ConnectionFactoryActor.VirtualConnectionSettings connectionSettings, Envelope.Adress adress, TMessage message, IActorRef from)
            : base(message, from)
        {
            ConnectionSettings = connectionSettings;
            Adress = adress;
        }

        public override ConnectionFactoryActor.VirtualConnectionSettings ConnectionSettings { get; }
        public override Envelope.Adress Adress { get; }
        public new TMessage Message => (TMessage)base.Message;
    }

    public abstract class Send
    {
        protected Send(object message, IActorRef from)
        {
            Message = message;
            From = @from;
        }

        public abstract ConnectionFactoryActor.VirtualConnectionSettings ConnectionSettings { get; }
        public abstract Envelope.Adress Adress { get; }
        public virtual object Message { get; }
        public IActorRef From { get; }
    }
}