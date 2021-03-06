﻿using System;
using Akka.Actor;
using Akka.Util;

namespace AMQPOverWebSocketProxy.Akka
{
    public struct TypedActorRef<T> : IActorRef<T>
    {
        public TypedActorRef(IActorRef aref) : this()
        {
            Ref = aref ?? throw new ArgumentNullException(nameof(aref), $"{this} has received null instead of {nameof(IActorRef)}");
        }

        public IActorRef Ref { get; }

        void ICanTell.Tell(object message, IActorRef sender) => Ref.Tell(message, sender);
        public void Tell(T message, IActorRef sender) => Ref.Tell(message, sender);
        public void Tell(T message) => Ref.Tell(message, ActorCell.GetCurrentSelfOrNoSender());

        public bool Equals(IActorRef other) =>
            other is TypedActorRef<T> ? Ref.Equals(((TypedActorRef<T>)other).Ref) : Ref.Equals(other);

        public int CompareTo(IActorRef other) =>
            other is TypedActorRef<T> ? Ref.CompareTo(((TypedActorRef<T>)other).Ref) : Ref.CompareTo(other);

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
}