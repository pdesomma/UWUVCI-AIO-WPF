using System;
using System.Collections.Generic;

namespace WiiUInjector.Messaging
{
    /// <summary>
    /// Sends messages to subscribed members.
    /// </summary>
    public static class Messenger
    {
        private static readonly Dictionary<Type, List<object>> s_subscriptions = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Register an object for notifications of a certain type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public static void Register<T>(Action<T> handler)
        {
            if (handler is null) return;

            if(!s_subscriptions.ContainsKey(typeof(T))) s_subscriptions.Add(typeof(T), new List<object>());
            s_subscriptions[typeof(T)].Add(handler);
        }

        /// <summary>
        /// Sends a notification to subscribed members.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public static void Send<T>(T message)
        {
            if(s_subscriptions.ContainsKey(typeof(T))) 
            {
                foreach(var subscriber in s_subscriptions[typeof(T)])
                {
                    (subscriber as Action<T>)(message);
                }
            }
        }
    }
}
