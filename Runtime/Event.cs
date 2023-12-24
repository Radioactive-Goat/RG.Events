using System;
using System.Collections.Generic;
using System.Reflection;

namespace RG.Events
{
    public abstract class Event<T> : IEvent<T> where T : IEventArgs
    {
        private Action<T> _subscribers;

#if UNITY_EDITOR
        private List<string> _strigizedSubscribers = new List<string>();
        List<string> IEvent.Subscribers { get => _strigizedSubscribers; }
#endif

        public void Subscribe(Action<T> subscriber)
        {
            _subscribers += subscriber;
#if UNITY_EDITOR
            _strigizedSubscribers.Add(subscriber.GetMethodInfo().DeclaringType.FullName + "." + subscriber.GetMethodInfo().Name);
#endif
        }

        public void Unsubscribe(Action<T> subscriber) 
        {
            _subscribers -= subscriber;
#if UNITY_EDITOR
            _strigizedSubscribers.Remove(subscriber.GetMethodInfo().DeclaringType.FullName + "." + subscriber.GetMethodInfo().Name);
#endif
        }

        public void Invoke(T args)
        {
            if (_subscribers != null)
            {
                _subscribers.Invoke(args);
            }

#if UNITY_EDITOR
            EventSystem.Instance.AddToInvokeStack(new InvokationMetaData
            {
                EventName = GetType().FullName,
                TimeStamp = DateTime.Now.ToString("T"),
                ArgumentData = args
            });
#endif
        }
    }
}