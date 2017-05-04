﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Util;
using AMQPOverWebSocketProxy.WebSocket.Messages;
using Common.Serialization;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public abstract class BaseAmqpCommand<TMessage> : ISubCommand<WebSocketSession>
    {
        public void ExecuteCommand(WebSocketSession session, SubRequestInfo requestInfo)
        {
            AmqpRequest<TMessage> amqpRequest;
            try
            {
                amqpRequest =
                    (AmqpRequest<TMessage>)session.AppServer.JsonDeserialize(requestInfo.Body, typeof(AmqpRequest<TMessage>));
            }
            catch (Exception ex)
            {
                session.Send(session.AppServer.JsonSerialize(new FormatErrorMessage($"Incorrectly formatted message. {ex.Message}")));
                return;
            }
            Execute(session, amqpRequest);
        }

        protected abstract void Execute(WebSocketSession session, AmqpRequest<TMessage> request);

        public abstract string Name { get; }
    }


    public class SendAmqpCommand2 : Command2<AmqpRequest<object>>
    {
        public SendAmqpCommand2(IRequestActorFactory actorFactory) : base(actorFactory)
        {
        }

        public override string Name { get; } = "Send";
    }

    public abstract class Command2<TRequest> : ISubCommand<WebSocketSession>
    {
        private readonly IRequestActorFactory _actorFactory;

        protected Command2(IRequestActorFactory actorFactory)
        {
            _actorFactory = actorFactory;
        }

        public void ExecuteCommand(WebSocketSession session, SubRequestInfo requestInfo)
        {
            var actor = _actorFactory.Create<TRequest>(session);

            actor.Tell(new SubRequestReceived<TRequest>(requestInfo));            
        }

        public abstract string Name { get; }
    }

    public abstract class Command<TRequest> : ISubCommand<WebSocketSession>
    {
        private readonly IActorRefGeneric<TRequest> _subRequestActor;

        protected Command(IActorRefGeneric<TRequest> subRequestActor)
        {
            _subRequestActor = subRequestActor;
        }

        public void ExecuteCommand(WebSocketSession session, SubRequestInfo requestInfo)
        {

            _subRequestActor.Tell(new SubRequestReceived<TRequest>(requestInfo));
            if (_subRequestActor.Ask<CommandProcessed>(new GetCommandProcessStatus()).Wait(TimeSpan.FromSeconds(5)) ==
                false)
            {
                session.Send(session.AppServer.JsonSerialize(new Timedout()));
            }
        }

        public abstract string Name { get; }
    }

    public interface IRequestActorFactory
    {
        IActorRefGeneric<TRequest> Create<TRequest>(WebSocketSession session);
    }

    public class RequestActorFactory : IRequestActorFactory
    {
        private readonly ActorSystem _system;
        private readonly ISerializer _serializer;

        public RequestActorFactory(ActorSystem system, ISerializer serializer)
        {
            _system = system;
            _serializer = serializer;
        }

        public IActorRefGeneric<TRequest> Create<TRequest>(WebSocketSession session)
        {
            var sessionSenderActor = _system.ActorOf(Props.Create(() => new SessionSenderActor(session)));
            return _system.ActorOf(
                Props<TRequest>.Create(() => new SubRequestActor<TRequest>(_serializer, sessionSenderActor)));
        }
    }

    public class GetCommandProcessStatus
    {
    }

    public class Timedout
    {
    }

    public class SessionSenderActor : ReceiveActor
    {
        public SessionSenderActor(WebSocketSession session)
        {
            Receive<SendMessage>(message => session.Send(session.AppServer.JsonSerialize(message)));
        }
    }

    public class SendMessage
    {
        public SendMessage(object message)
        {
            Message = message;
        }

        public object Message { get; }
    }

    public class SubRequestActor<TRequest> : ReceiveActor
    {
        private readonly IActorRef _sessionActor;
        private readonly ISerializer _serializer;

        public SubRequestActor(ISerializer serializer, IActorRef sessionActor)
        {
            _sessionActor = sessionActor;
            _serializer = serializer;
            Receive<SubRequestReceived<TRequest>>(request => Handler(request));
        }

        private void Handler(SubRequestReceived<TRequest> subRequestReceived)
        {
            TRequest amqpRequest;
            try
            {
                amqpRequest = _serializer.DeserializeFromString<TRequest>(subRequestReceived.RequestInfo.Body);
            }
            catch (Exception ex)
            {
                _sessionActor.Tell(new SendMessage(new FormatErrorMessage($"Incorrectly formatted message. {ex.Message}")));
                return;
            }
            // todo: send to another actor

            Sender.Tell(new SubRequestParsed<TRequest>(amqpRequest));

            // todo: tell sender when everything went fine
            Sender.Tell(new CommandProcessed());
        }
    }

    internal class CommandProcessed
    {

    }



    internal class SubRequestParsed<T>
    {
        public T AmqpRequest { get; }

        public SubRequestParsed(T amqpRequest)
        {
            AmqpRequest = amqpRequest;
        }
    }

    public class SubRequestReceived<T>
    {
        public SubRequestInfo RequestInfo { get; }

        public SubRequestReceived(SubRequestInfo requestInfo)
        {
            RequestInfo = requestInfo;
        }
    }

    public class CommandSupervisingActor : ReceiveActor
    {

    }


    public struct TypedRef<T> : IActorRefGeneric<T>
    {
        public TypedRef(IActorRef aref) : this()
        {
            if (aref == null) throw new ArgumentNullException(nameof(aref), $"{this} has received null instead of {nameof(IActorRef)}");
            Ref = aref;
        }

        public IActorRef Ref { get; }

        void ICanTell.Tell(object message, IActorRef sender) => Ref.Tell(message, sender);
        public void Tell(T message, IActorRef sender) => Ref.Tell(message, sender);
        public void Tell(T message) => Ref.Tell(message, ActorCell.GetCurrentSelfOrNoSender());

        public bool Equals(IActorRef other) =>
            other is TypedRef<T> ? Ref.Equals(((TypedRef<T>)other).Ref) : Ref.Equals(other);

        public int CompareTo(IActorRef other) =>
            other is TypedRef<T> ? Ref.CompareTo(((TypedRef<T>)other).Ref) : Ref.CompareTo(other);

        public ISurrogate ToSurrogate(ActorSystem system) => new TypeRefSurrogate<T>(Ref.ToSurrogate(system));

        public int CompareTo(object obj)
        {
            if (obj is IActorRef) return CompareTo((IActorRef)obj);
            throw new ArgumentException($"Cannot compare {obj} to {this}");
        }

        public ActorPath Path => Ref.Path;

        public override int GetHashCode() => Ref.GetHashCode();
        public override bool Equals(object obj) => obj is IActorRef && Equals((IActorRef)obj);
        public override string ToString() => Ref.ToString();
    }

    public interface IActorRefGeneric<in T> : IActorRef
    {
        void Tell(T message, IActorRef sender);
        void Tell(T message);
    }

    public struct Props<T> : ISurrogated
    {
        public readonly Props Underlying;

        public Props(Props props) : this()
        {
            if (props == null) throw new ArgumentNullException(nameof(props), $"{this} has received null instead of {nameof(Props)}");
            this.Underlying = props;
        }

        public static Props<T> Create<TActor>(Expression<Func<TActor>> fac) where TActor : ActorBase
            => new Props<T>(Props.Create(fac));

        public ISurrogate ToSurrogate(ActorSystem system) => new TypedPropsSurrogate<T>(Underlying.ToSurrogate(system));
    }

    internal struct TypedPropsSurrogate<T> : ISurrogate
    {
        public readonly ISurrogate PropsSurrogate;

        public TypedPropsSurrogate(ISurrogate propsSurrogate) : this()
        {
            PropsSurrogate = propsSurrogate;
        }

        public ISurrogated FromSurrogate(ActorSystem system) => new Props<T>((Props)PropsSurrogate.FromSurrogate(system));
    }


    internal struct TypeRefSurrogate<T> : ISurrogate
    {
        public readonly ISurrogate RefSurrogate;

        public TypeRefSurrogate(ISurrogate refSurrogate) : this()
        {
            RefSurrogate = refSurrogate;
        }

        public ISurrogated FromSurrogate(ActorSystem system) => new TypedRef<T>((IActorRef)RefSurrogate.FromSurrogate(system));
    }

    public abstract class Actor<T> : UntypedActor
    {
        protected abstract void OnReceive(T message);

        protected override void OnReceive(object message)
        {
            if (message is T)
                OnReceive((T)message);
            else Unhandled(message);
        }
    }

    public static class TypedExtensions
    {
        public static TypedRef<T> ActorOf<T>(this IActorRefFactory fac, Props<T> props) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying));

        public static TypedRef<T> ActorOf<T>(this IActorRefFactory fac, Props<T> props, string name) =>
            new TypedRef<T>(fac.ActorOf(props.Underlying, name));

        public static TypedRef<T> Watch<T>(this IActorContext ctx, TypedRef<T> tref)
        {
            ctx.Watch(tref.Ref);
            return tref;
        }

        public static TypedRef<T> Unwatch<T>(this IActorContext ctx, TypedRef<T> tref)
        {
            ctx.Unwatch(tref.Ref);
            return tref;
        }
    }
}