using System;
using Akka.Actor;
using AMQPOverWebSocketProxy.Akka;

namespace AMQPOverWebSocketProxy.Actors
{
    public class MessageProxyActor<TReceive, TSend> : ReceiveActor<TReceive>
    {
        private readonly IActorRef<TSend> _receiver;
        private readonly Func<TReceive, TSend> _mapper;
        private readonly IActorRef _from;

        public MessageProxyActor(IActorRef<TSend> receiver, Func<TReceive, TSend> mapper, IActorRef from)
        {
            _receiver = receiver;
            _mapper = mapper;
            _from = @from;
        }

        protected override void OnReceive(TReceive message)
        {
            _receiver.Tell(_mapper(message), _from);
        }
    }
}