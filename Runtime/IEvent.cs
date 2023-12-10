using System;
using System.Collections.Generic;

namespace RG.Events
{
    public interface IEvent
    {
#if UNITY_EDITOR
        internal List<string> Callbacks { get; }
#endif
    }

    public interface IEvent<T> : IEvent where T : IEventArgs
    {
        void AddCallback(Action<T> callback);
        void Invoke(T args);
    }
}