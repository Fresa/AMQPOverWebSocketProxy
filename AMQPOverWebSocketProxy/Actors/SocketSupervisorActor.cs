using System;
using Akka.Actor;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy.Actors
{
    public class SocketSupervisorActor : ReceiveActor
    {
        public SocketSupervisorActor()
        {
            Receive<SocketActor.FailedStartingService>(service => Context.Parent.Tell(service));

            Context.ActorOf(Context.DI().Props<SocketActor>(), "socket-actor");
        }
    }
}