using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;

namespace CMA
{
    public class Compositor<K> : ICompositor<K>
    {
        protected Dictionary<K, List<IComponent<K>>> Cache = new Dictionary<K, List<IComponent<K>>>();
        protected List<IComponent<K>> Components = new List<IComponent<K>>();
        protected IMessageManager MessageManager;

        public Compositor()
        {
            MessageManager = new MessageManager();
        }

        public Compositor(IMessageManager manager)
        {
            MessageManager = manager;
        }

        #region Compositor

        public virtual bool Contains(K key)
        {
            return Cache.ContainsKey(key);
        }

        public bool Contains<T>(T component) where T : IComponent<K>
        {
            return Contains(component.Key);
        }

        public bool Contains<T>(K key, T component) where T : IComponent<K>
        {
            return Contains(key) && Cache[key].Contains(component);
        }

        public virtual void AddComponent<T>(T component) where T : IComponent<K>
        {
            if (Contains(component.Key))
            {
                if (!Cache[component.Key].Contains(component))
                {
                    Cache[component.Key].Add(component);
                    Components.Add(component);
                    component.OnAdd(this);
                    SubscribeComponent(component);
                }
            }
            else
            {
                Cache.Add(component.Key, new List<IComponent<K>> { component });
                Components.Add(component);
                component.OnAdd(this);
                SubscribeComponent(component);
            }
        }


        public void RemoveComponent<T>(T component) where T : IComponent<K>
        {
            if (Contains(component))
            {
                Components.Remove(component);
                Cache[component.Key].Remove(component);
                UnsubscribeComponent(component);
            }
        }

        public virtual T GetComponent<T>(K key)
        {
            var result = default(T);

            if (Contains(key))
                result = (T)Cache[key][0];

            return result;
        }

        /// <summary>
        /// Return first available component who can be cast to type T
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <returns></returns>
        public virtual T GetComponent<T>()
        {
            var result = default(T);

            foreach (var component in Components)
            {
                if (component is T)
                {
                    result = (T) component;
                    break;
                }
            }

            return result;
        }

        public virtual List<T> GetComponents<T>(K key)
        {
            List<T> result = null;

            if (Contains(key))
                result = Cache[key] as List<T>;

            return result;
        }

        #endregion

        #region Messages

        public virtual void AddRequestMarker(IRequestMarkerHandler handler)
        {
            MessageManager.AddRequestMarker(handler);
        }

        public virtual void RemoveRequestMarker(IRequestMarkerHandler handler)
        {
            MessageManager.RemoveRequestMarker(handler);
        }

        public virtual void AddMessageMarker(IMessageMarkerHandler handler)
        {
            MessageManager.AddMessageMarker(handler);
        }

        public virtual void RemoveMessageMarker(IMessageMarkerHandler handler)
        {
            MessageManager.RemoveMessageMarker(handler);
        }

        public IMessageManager NewWithType()
        {
            return new Compositor<K>();
        }

        public virtual void SendMessage(IMessage message)
        {
            MessageManager.SendMessage(message);
        }

        public virtual bool ContainsMessage<T>() where T : IMessage
        {
            return MessageManager.ContainsMessage<T>();
        }

        public virtual bool ContainsMessage(IMessage message)
        {
            return MessageManager.ContainsMessage(message);
        }

        protected virtual void SubscribeComponent<T>(T component) where T : IComponent<K>
        {
            foreach (var message in component.ToOwnerMessages)
                SubscribeMessageReciever(message);

            foreach (var request in component.ToOwnerRequests)
                SubscribeRequestReciever(request);

            foreach (var marker in component.ToOwnerMessageMarkers)
                AddMessageMarker(marker);

            foreach (var marker in component.ToOwnerRequestMarkers)
                AddRequestMarker(marker);
        }

        protected virtual void UnsubscribeComponent<T>(T component) where T : IComponent<K>
        {
            foreach (var message in component.ToOwnerMessages)
                RemoveMessageReciever(message);

            foreach (var request in component.ToOwnerRequests)
                RemoveRequestReciever(request);
        }

        public virtual void SubscribeMessageReciever(IMessageHandler handler)
        {
            MessageManager.SubscribeMessageReciever(handler);
        }

        public virtual void SubscribeMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            MessageManager.SubscribeMessageReciever(@delegate);
        }

        public virtual void SubscribeMessageReciever<K1, T>(MessageDelegate<T> @delegate) where K1 : IMessage
            where T : IMessage
        {
            MessageManager.SubscribeMessageReciever<K1, T>(@delegate);
        }

        public virtual void RemoveMessageReciever(IMessageHandler handler)
        {
            MessageManager.RemoveMessageReciever(handler);
        }

        public virtual void RemoveMessageReciever<T>() where T : IMessage
        {
            MessageManager.RemoveMessageReciever<T>();
        }

        public virtual void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            MessageManager.RemoveMessageReciever(@delegate);
        }

        public virtual void RemoveMessageReciever<K1, T>(MessageDelegate<T> @delegate) where K1 : IMessage
            where T : IMessage
        {
            MessageManager.RemoveMessageReciever<K1, T>(@delegate);
        }

        public virtual object SendRequest(IRequest request)
        {
            return MessageManager.SendRequest(request);
        }

        public virtual T SendRequest<T>(IRequest request)
        {
            return MessageManager.SendRequest<T>(request);
        }

        public virtual T SendRequest<T>()
        {
            return MessageManager.SendRequest<T>();
        }

        public bool ContainsRequest<T>()
        {
            return MessageManager.ContainsRequest<T>();
        }

        public bool ContainsRequest(IRequest request)
        {
            return MessageManager.ContainsRequest(request);
        }

        public virtual void SubscribeRequestReciever(IRequestHandler handler)
        {
            MessageManager.SubscribeRequestReciever(handler);
        }

        public void SubscribeRequestReciever<T>(RequestSimpleDelegate<T> @delegate)
        {
            MessageManager.SubscribeRequestReciever(@delegate);
        }

        public void SubscribeRequestReciever<T, K1>(RequestDelegate<T, K1> @delegate) where K1 : IRequest
        {
            MessageManager.SubscribeRequestReciever<T, K1>(@delegate);
        }

        public virtual void RemoveRequestReciever(IRequestHandler handler)
        {
            MessageManager.RemoveRequestReciever(handler);
        }

        public void RemoveRequestReciever<T>(RequestSimpleDelegate<T> @delegate)
        {
            MessageManager.RemoveRequestReciever(@delegate);
        }

        public void RemoveRequestReciever<T, K1>(RequestDelegate<T, K1> @delegate) where K1 : IRequest
        {
            MessageManager.RemoveRequestReciever<T, K1>(@delegate);
        }

        #endregion
    }
}