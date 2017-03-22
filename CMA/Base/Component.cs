using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;
using CMA.Messages.Mediators;

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

            ToOwnerMessages = new List<IMessageMediator>();
            ToGlobalMessages = new List<IMessageMediator>();

            ToOwnerRequests = new List<IRequestMediator>();
            ToGlobalRequests = new List<IRequestMediator>();

            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        private Component(IMessageManager messageManager)
        {
            MessageManager = messageManager;
        }

        public List<IMessageMediator> ToGlobalMessages { get; protected set; }
        public List<IRequestMediator> ToGlobalRequests { get; protected set; }
        public List<IMessageMediator> ToOwnerMessages { get; protected set; }
        public List<IRequestMediator> ToOwnerRequests { get; protected set; }
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
            return MessageManager.SendRequest<T1>(request);
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

        protected virtual void SubscribeMessage<T>(MessageDelegate<T> @delegate, BindType bindType = BindType.ToOwner)
            where T : IMessage
        {
            MessageManager.SubscribeMessage(new MessageHandler<T>(@delegate));

            var mediator = new MessageMediator<T>(MessageManager);

            if (bindType == BindType.ToOwner)
                ToOwnerMessages.Add(mediator);
            else
                ToGlobalMessages.Add(mediator);
        }

        protected virtual void SubscribeRequest<T>(RequestSimpleDelegate<T> @delegate,
            BindType bindType = BindType.ToOwner)
        {
            MessageManager.SubscribeRequest(new RequestSimpleHandler<T>(@delegate));

            var handler = new SimpleRequestMediator<T>(MessageManager);

            if (bindType == BindType.ToOwner)
                ToOwnerRequests.Add(handler);
            else
                ToGlobalRequests.Add(handler);
        }

        protected virtual void SubscribeRequest<T, K>(RequestDelegate<T, K> @delegate,
            BindType bindType = BindType.ToOwner) where K : IRequest
        {
            MessageManager.SubscribeRequest(new RequestHandler<T, K>(@delegate));
            var handler = new RequestMediatorM<T, K>(MessageManager);

            if (bindType == BindType.ToOwner)
                ToOwnerRequests.Add(handler);
            else
                ToGlobalRequests.Add(handler);
        }

        protected virtual void Subscribe()
        {
        }
    }
}