using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;

namespace CMA
{
    public abstract class Component<T> : IComponent<T>
    {
        protected IMessageManager MessageManager;

        protected Component(T key) : this(key, new MessageManager())
        {
        }

        protected Component(T key, IMessageManager messageManager) : this(messageManager)
        {
            Key = key;

            ToOwnerMessages = new List<IMessageHandler>();
            ToOwnerRequests = new List<IRequestHandler>();

            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        private Component(IMessageManager messageManager)
        {
            MessageManager = messageManager;
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

        public void SendMessage(IMessage message)
        {
            MessageManager.SendMessage(message);
        }

        public bool ContainsMessage<T1>() where T1 : IMessage
        {
            return MessageManager.ContainsMessage<T1>();
        }

        public bool ContainsMessage(IMessage message)
        {
            return MessageManager.ContainsMessage(message);
        }

        public object SendRequest(IRequest request)
        {
            return MessageManager.SendRequest(request);
        }

        public T1 SendRequest<T1>(IRequest request)
        {
            return (T1) MessageManager.SendRequest(request);
        }

        public T1 SendRequest<T1>()
        {
            return MessageManager.SendRequest<T1>();
        }

        public bool ContainsRequest<T1>()
        {
            return MessageManager.ContainsRequest<T1>();
        }

        public bool ContainsRequest(IRequest request)
        {
            return MessageManager.ContainsRequest(request);
        }

        protected virtual void AddMessageMaker<T>(MessageMarkerDelegate<IMessage, T> @delegate) where T : IMarker
        {
            ToOwnerMessageMarkers.Add(new MessageMarkerHandler<T>(@delegate));
        }

        protected virtual void AddRequestMarker<T>(RequestMarkerDelegate<object, IRequest, T> @delegate)
            where T : IMarker
        {
            ToOwnerRequestMarkers.Add(new RequestMarkerHandler<T>(@delegate));
        }

        protected virtual void SubscribeMessageRecieverToOwner<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            MessageManager.SubscribeMessageReciever(new MessageHandler<T>(@delegate));
            ToOwnerMessages.Add(new MessageHandler<T>(delegate(T message) { MessageManager.SendMessage(message); }));
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

        protected virtual void Subscribe()
        {
        }
    }
}