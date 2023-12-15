using System;
using System.Collections.Generic;
using System.Reflection;

namespace RG.Events
{
    public abstract class Event<T> : IEvent<T> where T : IEventArgs
    {
        private Action<T> _callbacks;

#if UNITY_EDITOR
        private List<string> _strigizedCallbacks = new List<string>();
        List<string> IEvent.Callbacks { get => _strigizedCallbacks; }
#endif

        public void AddCallback(Action<T> callback)
        {
            _callbacks += callback;
#if UNITY_EDITOR
            _strigizedCallbacks.Add(callback.GetMethodInfo().DeclaringType.FullName + "." + callback.GetMethodInfo().Name);
#endif
        }

        public void Invoke(T args)
        {
            _callbacks.Invoke(args);

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