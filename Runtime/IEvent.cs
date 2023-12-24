using System;
using System.Collections.Generic;

namespace RG.Events
{
    public interface IEvent
    {
#if UNITY_EDITOR
        internal List<string> Subscribers { get; }
#endif
    }

    public interface IEvent<T> : IEvent where T : IEventArgs
    {
        void Subscribe(Action<T> subscriber);
        void Unsubscribe(Action<T> subscriber);
        void Invoke(T args);
    }
}