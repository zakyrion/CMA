using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;
using CMA.Messages.Mediators;

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

        public virtual IMessageManager System { get; protected set; }

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
                }
            }
            else
            {
                Cache.Add(component.Key, new List<IComponent<K>> {component});
                Components.Add(component);
                component.OnAdd(this);
            }
        }


        public void RemoveComponent<T>(T component) where T : IComponent<K>
        {
            if (Contains(component))
            {
                Components.Remove(component);
                component.OnRemove();
                Cache[component.Key].Remove(component);
            }
        }

        public virtual T GetComponent<T>(K key)
        {
            var result = default(T);

            if (Contains(key) && Cache[key][0] is T)
                result = (T) Cache[key][0];

            return result;
        }

        /// <summary>
        ///     Return first available component who can be cast to type T
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

        public virtual void Quit()
        {
            foreach (var component in Components)
                component.Quit();

            var components = new List<IComponent<K>>(Components);

            foreach (var component in components)
                RemoveComponent(component);

            MessageManager.Quit();
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

        public virtual void SubscribeMessage(IMessageHandler handler)
        {
            MessageManager.SubscribeMessage(handler);
        }

        public virtual void SubscribeMessage<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            MessageManager.SubscribeMessage(@delegate);
        }

        public virtual void SubscribeMessage<K1, T>(MessageDelegate<T> @delegate) where K1 : IMessage
            where T : IMessage
        {
            MessageManager.SubscribeMessage<K1, T>(@delegate);
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
            if (System.ContainsRequest(request))
                return System.SendRequest(request);

            return MessageManager.SendRequest(request);
        }

        public virtual T SendRequest<T>(IRequest request)
        {
            if (System.ContainsRequest(request))
                return System.SendRequest<T>(request);

            return MessageManager.SendRequest<T>(request);
        }

        public virtual T SendRequest<T>()
        {
            if (System.ContainsRequest<T>())
                return System.SendRequest<T>();

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

        public virtual void SubscribeRequest(IRequestHandler handler)
        {
            MessageManager.SubscribeRequest(handler);
        }

        public void SubscribeRequest<T>(RequestSimpleDelegate<T> @delegate)
        {
            MessageManager.SubscribeRequest(@delegate);
        }

        public void SubscribeRequest<T, K1>(RequestDelegate<T, K1> @delegate) where K1 : IRequest
        {
            MessageManager.SubscribeRequest(@delegate);
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
            MessageManager.RemoveRequestReciever(@delegate);
        }

        public void SubscribeMediator(IMessageMediator mediator)
        {
            MessageManager.SubscribeMediator(mediator);
        }

        public void SubscribeMediator(IRequestMediator mediator)
        {
            MessageManager.SubscribeMediator(mediator);
        }

        public void RemoveMediator(IRequestMediator mediator)
        {
            MessageManager.RemoveMediator(mediator);
        }

        public void RemoveMediator(IMessageMediator mediator)
        {
            MessageManager.RemoveMediator(mediator);
        }

        #endregion
    }
}