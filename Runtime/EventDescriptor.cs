using System;
using System.Collections.Generic;
using System.Reflection;

namespace RG.Events
{
    internal class EventDescriptor
    {
        private Action<IEvent> _subscribers;

#if UNITY_EDITOR
        private List<string> _strigizedSubscribers = new List<string>();
        internal List<string> StringizedSubscribers => _strigizedSubscribers;
#endif

        internal void Subscribe(Action<IEvent> subscriber)
        {
            _subscribers += subscriber;
#if UNITY_EDITOR
            _strigizedSubscribers.Add(subscriber.GetMethodInfo().DeclaringType.FullName + "." + subscriber.GetMethodInfo().Name);
#endif
        }

        public void Unsubscribe(Action<IEvent> subscriber) 
        {
            _subscribers -= subscriber;
#if UNITY_EDITOR
            _strigizedSubscribers.Remove(subscriber.GetMethodInfo().DeclaringType.FullName + "." + subscriber.GetMethodInfo().Name);
#endif
        }

        public void Invoke(IEvent newEvent)
        {
            if (_subscribers != null)
            {
                _subscribers.Invoke(newEvent);
            }

#if UNITY_EDITOR
            EventSystem.Instance.AddToInvokeStack(new InvokationMetaData
            {
                EventName = newEvent.GetType().FullName,
                TimeStamp = DateTime.Now.ToString("T"),
                ArgumentData = newEvent
            });
#endif
        }
    }
}