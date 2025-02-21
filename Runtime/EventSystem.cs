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

#if UNITY_EDITOR
        Dictionary<Type, EventDescriptor> IEventSystem.Events => _events;

        uint IEventSystem.InvokeStackBufferSize { get => _invokeStackBufferSize; set => _invokeStackBufferSize = value; }

        List<InvokationMetaData> IEventSystem.InvokeStack => _invokeStack;

        UnityEvent<InvokationMetaData> IEventSystem.OnEventInvoked => _eventInvoked;

        private uint _invokeStackBufferSize = 100u;

        private List<InvokationMetaData> _invokeStack = new List<InvokationMetaData>();

        private UnityEvent<InvokationMetaData> _eventInvoked = new UnityEvent<InvokationMetaData>();
#endif

        private Dictionary<Type, EventDescriptor> _events = new Dictionary<Type, EventDescriptor>();


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

            _events[typeof(T)] = new EventDescriptor();
        }

        public void DeRegister<T>() where T : IEvent
        {
            _events.Remove(typeof(T));
        }

        public void Invoke<T>(T newEvent) where T : IEvent
        {
            _events[typeof(T)].Invoke(newEvent);
        }

        public void Subscribe<T>(Action<IEvent> sub) where T : IEvent
        {
            _events[typeof(T)].Subscribe(sub);
        }

        public void Unsubscribe<T>(Action<IEvent> sub) where T : IEvent
        {
            _events[typeof(T)].Unsubscribe(sub);
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