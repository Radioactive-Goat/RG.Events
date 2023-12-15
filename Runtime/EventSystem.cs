using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[assembly: InternalsVisibleTo("com.radioactivegoat.events.editor")]

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
    public class EventSystem : MonoBehaviour
    {
        private static EventSystem _instance;
        public static EventSystem Instance => _instance;

        private Dictionary<Type, IEvent> _events = new Dictionary<Type, IEvent>();

#if UNITY_EDITOR
        internal Dictionary<Type, IEvent> Events { get {  return _events; } }
        internal uint invokeStackBufferSize = 100;
        internal List<InvokationMetaData> invokeStack = new List<InvokationMetaData>();
        internal UnityEvent<InvokationMetaData> invokeEvent = new UnityEvent<InvokationMetaData>();
#endif

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

        public void AddCallback<TEvent, TEventArgs>(Action<TEventArgs> callback) where TEvent : IEvent where TEventArgs : IEventArgs
        {
            var evnt = (IEvent<TEventArgs>)_events[typeof(TEvent)];
            evnt.AddCallback(callback);
        }

#if UNITY_EDITOR
        internal void AddToInvokeStack(InvokationMetaData data)
        {
            if(invokeStack.Count > invokeStackBufferSize) 
            {
                invokeStack.RemoveAt(0);
            }

            invokeStack.Add(data);

            invokeEvent.Invoke(data);
        }
#endif
    }
}