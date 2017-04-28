using System;
using Akka.Actor;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy.Actors
{
    public class SocketSupervisor : ReceiveActor
    {
        public SocketSupervisor(Action terminate)
        {
            Receive<SocketActor.FailedStartingService>(service => terminate());

            Context.ActorOf(Context.DI().Props<SocketActor>(), "socket-actor");
        }
    }
}