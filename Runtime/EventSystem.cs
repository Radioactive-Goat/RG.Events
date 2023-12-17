using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[assembly: InternalsVisibleTo("com.radioactivegoat.events.editor")]

namespace RG.Events
{
    public class EventSystem : MonoBehaviour, IEventSystem
    {
        private static EventSystem _instance;
        public static IEventSystem Instance => _instance;

        Dictionary<Type, IEvent> IEventSystem.Events => _events;

        uint IEventSystem.InvokeStackBufferSize { get => _invokeStackBufferSize; set => _invokeStackBufferSize = value; }

        List<InvokationMetaData> IEventSystem.InvokeStack => _invokeStack;

        UnityEvent<InvokationMetaData> IEventSystem.OnEventInvoked => _eventInvoked;

        private Dictionary<Type, IEvent> _events = new Dictionary<Type, IEvent>();

        private uint _invokeStackBufferSize = 100u;

        private List<InvokationMetaData> _invokeStack = new List<InvokationMetaData>();

        private UnityEvent<InvokationMetaData> _eventInvoked = new UnityEvent<InvokationMetaData>();

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public void Register<T>(bool force = false) where T : IEvent, new()
        {
            if(_events.ContainsKey(typeof(T)) && !force)
            {
                return;
            }

            _events[typeof(T)] = new T();
        }

        public void DeRegister<T>() where T : IEvent
        {
            _events.Remove(typeof(T));
        }

        public T GetEvent<T>() where T : IEvent
        {
            return (T)_events[typeof(T)];
        }

        public void Invoke<TEvent, TEventArgs>(TEventArgs args) where TEvent : IEvent where TEventArgs : IEventArgs
        {
            var evnt = (IEvent<TEventArgs>)_events[typeof(TEvent)];
            evnt.Invoke(args);
        }

        public void Subscribe<TEvent, TEventArgs>(Action<TEventArgs> subscriber) where TEvent : IEvent where TEventArgs : IEventArgs
        {
            var evnt = (IEvent<TEventArgs>)_events[typeof(TEvent)];
            evnt.Subscribe(subscriber);
        }

#if UNITY_EDITOR
        void IEventSystem.AddToInvokeStack(InvokationMetaData data)
        {
            if(_invokeStack.Count > _invokeStackBufferSize) 
            {
                _invokeStack.RemoveAt(0);
            }

            _invokeStack.Add(data);

            _eventInvoked.Invoke(data);
        }
#endif
    }
}