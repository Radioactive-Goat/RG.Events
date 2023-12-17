using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace RG.Events
{
#if UNITY_EDITOR
    public struct InvokationMetaData
    {
        public string EventName;
        public string TimeStamp;
        public object ArgumentData;
    }
#endif

    public interface IEventSystem
    {
        public void Register<T>(bool force = false) where T : IEvent, new();
        public void DeRegister<T>() where T : IEvent;
        public T GetEvent<T>() where T : IEvent;
        public void Invoke<TEvent, TEventArgs>(TEventArgs args) where TEvent : IEvent where TEventArgs : IEventArgs;
        public void Subscribe<TEvent, TEventArgs>(Action<TEventArgs> subscriber) where TEvent : IEvent where TEventArgs : IEventArgs;

#if UNITY_EDITOR
        internal Dictionary<Type, IEvent> Events { get; }
        internal uint InvokeStackBufferSize { get; set; }
        internal List<InvokationMetaData> InvokeStack { get; }
        internal UnityEvent<InvokationMetaData> OnEventInvoked { get; }
        internal void AddToInvokeStack(InvokationMetaData data);
#endif
    }
}
