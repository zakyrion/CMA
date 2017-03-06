using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;

namespace CMA
{
    public abstract class Component<T> : IComponent<T>
    {
        protected Component(T key)
        {
            Key = key;

            ToOwnerMessages = new List<IMessageHandler>();
            ToOwnerRequests = new List<IRequestHandler>();

            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        public List<IMessageHandler> ToOwnerMessages { get; protected set; }
        public List<IRequestHandler> ToOwnerRequests { get; protected set; }
        public List<IMessageMarkerHandler> ToOwnerMessageMarkers { get; protected set; }
        public List<IRequestMarkerHandler> ToOwnerRequestMarkers { get; protected set; }
        public virtual T Key { get; protected set; }
        public virtual ICompositor<T> Owner { get; protected set; }

        public virtual void OnAdd(ICompositor<T> owner)
        {
            Owner = owner;
        }

        public virtual void OnRemove()
        {
        }

        public abstract object Clone();

        protected virtual void SubscribeMessageRecieverToOwner<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            ToOwnerMessages.Add(new MessageHandler<T>(@delegate));
        }

        protected virtual void SubscribeRequestRecieverToOwner<T>(RequestSimpleDelegate<T> @delegate)
        {
            ToOwnerRequests.Add(new RequestSimpleHandler<T>(@delegate));
        }

        protected virtual void AddMessageMaker<T>(MessageDelegate<IMessage> @delegate) where T : Marker
        {
            ToOwnerMessageMarkers.Add(new MessageMarkerHandler<T>(@delegate));
        }

        protected virtual void AddRequestMarker<T>(RequestDelegate<object, IRequest> @delegate)
        {
            ToOwnerRequestMarkers.Add(new RequestMarkerHandler<T>(@delegate));
        }

        protected virtual void SubscribeRequestRecieverToOwner<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest
        {
            ToOwnerRequests.Add(new RequestHandler<T, K>(@delegate));
        }

        protected virtual void Subscribe()
        {
        }
    }
}