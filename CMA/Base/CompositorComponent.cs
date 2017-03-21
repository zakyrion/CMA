using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;

namespace CMA
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">MarkerKey for components identefication</typeparam>
    /// <typeparam name="K">key for compositor</typeparam>
    public class CompositorComponent<T, K> : Compositor<T>, IComponent<K>
    {
        public CompositorComponent(K key)
        {
            Key = key;

            ToOwnerMessages = new List<IMessageHandler>();
            ToOwnerRequests = new List<IRequestHandler>();
            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        public CompositorComponent(K key, IMessageManager manager) : base(manager)
        {
            Key = key;

            ToOwnerMessages = new List<IMessageHandler>();
            ToOwnerRequests = new List<IRequestHandler>();
            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        public CompositorComponent(CompositorComponent<T, K> obj) : base(obj.MessageManager.NewWithType())
        {
            Key = obj.Key;

            ToOwnerMessages = new List<IMessageHandler>();
            ToOwnerRequests = new List<IRequestHandler>();
            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            foreach (var component in obj.Components)
                AddComponent((IComponent<T>)component.Clone());

            Subscribe();
        }

        protected virtual IMarker Marker { get; set; }

        public virtual bool IsRoot { get; protected set; }
        public List<IMessageHandler> ToOwnerMessages { get; protected set; }
        public List<IRequestHandler> ToOwnerRequests { get; protected set; }
        public List<IMessageMarkerHandler> ToOwnerMessageMarkers { get; protected set; }
        public List<IRequestMarkerHandler> ToOwnerRequestMarkers { get; protected set; }
        public K Key { get; protected set; }
        public ICompositor<K> Owner { get; private set; }

        public virtual void OnAdd(ICompositor<K> owner)
        {
            Owner = owner;
        }

        public virtual void OnRemove()
        {
        }

        public virtual object Clone()
        {
            return new CompositorComponent<T, K>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="M">Type of marker</typeparam>
        /// <param name="delegate"></param>
        protected virtual void AddMessageMarker<M>(MessageMarkerDelegate<IMessage, M> @delegate) where M : IMarker
        {
            ToOwnerMessageMarkers.Add(new MessageMarkerHandler<M>(@delegate));
        }

        /// <summary>
        /// Create Simple Marker without method
        /// </summary>
        /// <typeparam name="M"></typeparam>
        protected virtual void AddMessageMarker<M>() where M : IMarker
        {
            ToOwnerMessageMarkers.Add(new MessageMarkerHandler<M>((message, marker) =>
            {
                var component = GetComponent<IMessageRespounder>((T)marker.ObjKey);

                if (component != null)
                    component.SendMessage(message);
            }));
        }

        protected virtual void AddRequestMarker<T>(RequestMarkerDelegate<object, IRequest, T> @delegate) where T : IMarker
        {
            ToOwnerRequestMarkers.Add(new RequestMarkerHandler<T>(@delegate));
        }

        /// <summary>
        /// Create Simple Marker without method
        /// </summary>
        /// <typeparam name="M"></typeparam>
        protected virtual void AddRequestMarker<M>() where M : IMarker
        {
            ToOwnerRequestMarkers.Add(new RequestMarkerHandler<M>((request, marker) =>
            {
                var component = GetComponent<IMessageRespounder>((T)marker.ObjKey);
                return component != null ? component.SendRequest(request) : null;
            }));
        }

        protected virtual void Subscribe()
        {
        }

        protected virtual void SubscribeMessageRecieverToOwner<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            MessageManager.SubscribeMessageReciever(new MessageHandler<T>(@delegate));
            ToOwnerMessages.Add(new MessageHandler<T>(delegate (T message) { MessageManager.SendMessage(message); }));
        }

        protected virtual void SubscribeRequestRecieverToOwner<T>(RequestSimpleDelegate<T> @delegate)
        {
            MessageManager.SubscribeRequestReciever(new RequestSimpleHandler<T>(@delegate));
            ToOwnerRequests.Add(new RequestSimpleHandler<T>(() => MessageManager.SendRequest<T>()));
        }

        protected virtual void SubscribeRequestRecieverToOwner<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest
        {
            MessageManager.SubscribeRequestReciever(new RequestHandler<T, K>(@delegate));
            ToOwnerRequests.Add(new RequestHandler<T, K>(message => MessageManager.SendRequest<T>(message)));
        }

        public override T1 SendRequest<T1>(IRequest request)
        {
            var result = default(T1);

            if (ContainsRequest<T1>() || ContainsRequest(request))
                result = base.SendRequest<T1>(request);
            else if (Owner != null && !IsRoot)
                result = Owner.SendRequest<T1>(request);

            return result;
        }

        public override T1 SendRequest<T1>()
        {
            var result = default(T1);

            if (ContainsRequest<T1>())
                result = base.SendRequest<T1>();
            else if (Owner != null && !IsRoot)
                result = Owner.SendRequest<T1>();

            return result;
        }

        public override void SendMessage(IMessage message)
        {
            if (ContainsMessage(message))
                base.SendMessage(message);
            else if (Owner != null && !IsRoot)
            {
                if (Marker != null)
                    message.AddMarkerForReturn(Marker);

                Owner.SendMessage(message);
            }
        }
    }
}