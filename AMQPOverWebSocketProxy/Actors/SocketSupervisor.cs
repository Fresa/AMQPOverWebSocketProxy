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

            Context.ActorOf(Context.DI().Props<SocketActor>(), "supervisor");
        }
    }

    public class SocketSupervisor2 : ReceiveActor
    {
        public SocketSupervisor2()
        {
            Receive<SocketActor.FailedStartingService>(service => Context.Parent.Tell(service));

            Context.ActorOf(Context.DI().Props<SocketActor>(), "supervisor");
        }
    }

    public class RemoteSupervisor : ReceiveActor
    {
        public RemoteSupervisor(Action terminate)
        {
            Receive<SocketActor.FailedStartingService>(service => terminate());

            Context.ActorOf(Props.Create<SocketSupervisor2>(), "socket-actor");
        }
    }
}