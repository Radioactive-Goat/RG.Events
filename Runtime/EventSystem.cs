using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("com.radioactivegoat.events.editor")]

namespace RG.Events
{
#if UNITY_EDITOR
    public struct InvokationMetaData
    {
        public string EventName;
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
        internal int InvokeStackBufferSize { get; set; } = 50;
        internal List<InvokationMetaData> InvokeStack { get; set; } = new List<InvokationMetaData>();
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
            if(InvokeStack.Count > InvokeStackBufferSize) 
            {
                InvokeStack.RemoveAt(0);
            }

            InvokeStack.Add(data);
        }
#endif
    }
}