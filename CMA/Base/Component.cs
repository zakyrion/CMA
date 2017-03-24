using System.Collections.Generic;
using CMA.Messages;
using CMA.Messages.Mediators;

namespace CMA
{
    public abstract class Component<T> : IComponent<T>
    {
        private bool _isGlobalSubscribe;
        private bool _isOwnerSubscribe;

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
        public virtual T Key { get; protected set; }
        public virtual ICompositor<T> Owner { get; protected set; }

        public virtual void OnAdd(ICompositor<T> owner)
        {
            Owner = owner;

            if (!_isOwnerSubscribe)
            {
                _isOwnerSubscribe = true;
                foreach (var mediator in ToOwnerMessages)
                    Owner.SubscribeMediator(mediator);

                foreach (var mediator in ToOwnerRequests)
                    Owner.SubscribeMediator(mediator);
            }
        }

        public virtual void OnRemove()
        {
            if (_isOwnerSubscribe)
            {
                foreach (var mediator in ToOwnerMessages)
                    Owner.RemoveMediator(mediator);

                foreach (var mediator in ToOwnerRequests)
                    Owner.RemoveMediator(mediator);

                _isOwnerSubscribe = false;
            }

            RemoveGlobal();

            Owner = null;
        }

        public virtual void Quit()
        {
            MessageManager.Quit();
        }

        public virtual void SubscribeGlobal()
        {
            if (!_isGlobalSubscribe)
            {
                _isGlobalSubscribe = true;
                foreach (var mediator in ToGlobalMessages)
                    Owner.System.SubscribeMediator(mediator);

                foreach (var mediator in ToGlobalRequests)
                    Owner.System.SubscribeMediator(mediator);
            }
        }

        public virtual void RemoveGlobal()
        {
            if (_isGlobalSubscribe)
            {
                foreach (var mediator in ToGlobalMessages)
                    Owner.System.RemoveMediator(mediator);

                foreach (var mediator in ToGlobalRequests)
                    Owner.System.RemoveMediator(mediator);
                _isGlobalSubscribe = false;
            }
        }

        public abstract object Clone();

        public void SendMessage(IMessage message)
        {
            if (ContainsMessage(message))
                MessageManager.SendMessage(message);

            Owner.SendMessage(message);
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
            if (ContainsRequest(request))
                return MessageManager.SendRequest(request);

            return Owner.SendRequest(request);
        }

        public T1 SendRequest<T1>(IRequest request)
        {
            if (ContainsRequest(request))
                return MessageManager.SendRequest<T1>(request);

            return Owner.SendRequest<T1>(request);
        }

        public T1 SendRequest<T1>()
        {
            var request = new SimpleRequest<T1>();
            if (ContainsRequest(request))
                return MessageManager.SendRequest<T1>(request);

            return Owner.SendRequest<T1>(request);
        }

        public bool ContainsRequest<T1>()
        {
            return MessageManager.ContainsRequest<T1>();
        }

        public bool ContainsRequest(IRequest request)
        {
            return MessageManager.ContainsRequest(request);
        }

        protected virtual void SubscribeMessage<T>(MessageDelegate<T> @delegate, BindType bindType = BindType.ToOwner)
            where T : IMessage
        {
            MessageManager.SubscribeMessage(new MessageHandler<T>(@delegate));

            var mediator = new MessageMediator<T>(MessageManager);

            if (Owner != null)
            {
                if (bindType == BindType.ToOwner)
                    Owner.SubscribeMediator(mediator);
                else if (Owner.System != null)
                    Owner.System.SubscribeMediator(mediator);
            }

            if (bindType == BindType.ToOwner)
                ToOwnerMessages.Add(mediator);
            else
                ToGlobalMessages.Add(mediator);
        }

        protected virtual void SubscribeRequest<T>(RequestSimpleDelegate<T> @delegate,
            BindType bindType = BindType.ToOwner)
        {
            MessageManager.SubscribeRequest(new RequestSimpleHandler<T>(@delegate));

            var mediator = new SimpleRequestMediator<T>(MessageManager);

            if (Owner != null)
            {
                if (bindType == BindType.ToOwner)
                    Owner.SubscribeMediator(mediator);
                else if (Owner.System != null)
                    Owner.System.SubscribeMediator(mediator);
            }

            if (bindType == BindType.ToOwner)
                ToOwnerRequests.Add(mediator);
            else
                ToGlobalRequests.Add(mediator);
        }

        protected virtual void SubscribeRequest<T, K>(RequestDelegate<T, K> @delegate,
            BindType bindType = BindType.ToOwner) where K : IRequest
        {
            MessageManager.SubscribeRequest(new RequestHandler<T, K>(@delegate));
            var mediator = new RequestMediatorM<T, K>(MessageManager);

            if (Owner != null)
            {
                if (bindType == BindType.ToOwner)
                    Owner.SubscribeMediator(mediator);
                else if (Owner.System != null)
                    Owner.System.SubscribeMediator(mediator);
            }

            if (bindType == BindType.ToOwner)
                ToOwnerRequests.Add(mediator);
            else
                ToGlobalRequests.Add(mediator);
        }

        protected virtual void Subscribe()
        {
        }
    }
}