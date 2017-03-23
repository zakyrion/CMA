using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;
using CMA.Messages.Mediators;

namespace CMA
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">MarkerKey for components identefication</typeparam>
    /// <typeparam name="K">key for compositor</typeparam>
    public class CompositorComponent<T, K> : Compositor<T>, IComponent<K>
    {
        private bool _isGlobalSubscribe;
        private bool _isOwnerSubscribe;

        public CompositorComponent(K key)
        {
            Key = key;

            ToOwnerMessages = new List<IMessageMediator>();
            ToOwnerRequests = new List<IRequestMediator>();
            ToGlobalMessages = new List<IMessageMediator>();
            ToGlobalRequests = new List<IRequestMediator>();
            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        public CompositorComponent(K key, IMessageManager manager) : base(manager)
        {
            Key = key;

            ToOwnerMessages = new List<IMessageMediator>();
            ToOwnerRequests = new List<IRequestMediator>();
            ToGlobalMessages = new List<IMessageMediator>();
            ToGlobalRequests = new List<IRequestMediator>();
            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            Subscribe();
        }

        public CompositorComponent(CompositorComponent<T, K> obj) : base(obj.MessageManager.NewWithType())
        {
            Key = obj.Key;

            ToOwnerMessages = new List<IMessageMediator>();
            ToOwnerRequests = new List<IRequestMediator>();
            ToGlobalMessages = new List<IMessageMediator>();
            ToGlobalRequests = new List<IRequestMediator>();
            ToOwnerMessageMarkers = new List<IMessageMarkerHandler>();
            ToOwnerRequestMarkers = new List<IRequestMarkerHandler>();

            foreach (var component in obj.Components)
                AddComponent((IComponent<T>) component.Clone());

            Subscribe();
        }

        protected virtual IMarker Marker { get; set; }

        public virtual bool IsRoot { get; protected set; }
        public List<IMessageMediator> ToGlobalMessages { get; protected set; }
        public List<IRequestMediator> ToGlobalRequests { get; protected set; }
        public List<IMessageMediator> ToOwnerMessages { get; protected set; }
        public List<IRequestMediator> ToOwnerRequests { get; protected set; }
        public List<IMessageMarkerHandler> ToOwnerMessageMarkers { get; protected set; }
        public List<IRequestMarkerHandler> ToOwnerRequestMarkers { get; protected set; }
        public K Key { get; protected set; }
        public ICompositor<K> Owner { get; private set; }

        public virtual void OnAdd(ICompositor<K> owner)
        {
            Owner = owner;
            System = owner.System;

            if (System != null)
            {
                foreach (var component in Components)
                    component.SubscribeGlobal();
            }

            if (!_isOwnerSubscribe)
            {
                _isOwnerSubscribe = true;
                foreach (var mediator in ToOwnerMessages)
                    Owner.SubscribeMediator(mediator);

                foreach (var mediator in ToOwnerRequests)
                    Owner.SubscribeMediator(mediator);

                foreach (var marker in ToOwnerMessageMarkers)
                    Owner.AddMessageMarker(marker);

                foreach (var marker in ToOwnerRequestMarkers)
                    Owner.AddRequestMarker(marker);
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

                foreach (var marker in ToOwnerMessageMarkers)
                    Owner.RemoveMessageMarker(marker);

                foreach (var marker in ToOwnerRequestMarkers)
                    Owner.RemoveRequestMarker(marker);

                _isOwnerSubscribe = false;
            }

            RemoveGlobal();

            Owner = null;
        }

        public virtual void SubscribeGlobal()
        {
            foreach (var component in Components)
                component.SubscribeGlobal();

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
            foreach (var component in Components)
                component.RemoveGlobal();

            if (_isGlobalSubscribe)
            {
                foreach (var mediator in ToGlobalMessages)
                    Owner.System.RemoveMediator(mediator);

                foreach (var mediator in ToGlobalRequests)
                    Owner.System.RemoveMediator(mediator);
                _isGlobalSubscribe = false;
            }
        }

        public virtual object Clone()
        {
            return new CompositorComponent<T, K>(this);
        }

        public override T1 SendRequest<T1>(IRequest request)
        {
            var result = default(T1);
            if (System.ContainsRequest<T1>() || System.ContainsRequest(request))
                result = System.SendRequest<T1>(request);
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
            else
                message.Fail();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="M">Type of marker</typeparam>
        /// <param name="delegate"></param>
        protected virtual void AddMessageMarker<M>(MessageMarkerDelegate<IMessage, M> @delegate) where M : IMarker
        {
            ToOwnerMessageMarkers.Add(new MessageMarkerHandler<M>(@delegate));
        }

        /// <summary>
        ///     Create Simple Marker without method
        /// </summary>
        /// <typeparam name="M"></typeparam>
        protected virtual void AddMessageMarker<M>() where M : IMarker
        {
            ToOwnerMessageMarkers.Add(new MessageMarkerHandler<M>((message, marker) =>
            {
                var component = GetComponent<IMessageRespounder>((T) marker.ObjKey);

                if (component != null)
                    component.SendMessage(message);
            }));
        }

        protected virtual void AddRequestMarker<T>(RequestMarkerDelegate<object, IRequest, T> @delegate)
            where T : IMarker
        {
            ToOwnerRequestMarkers.Add(new RequestMarkerHandler<T>(@delegate));
        }

        /// <summary>
        ///     Create Simple Marker without method
        /// </summary>
        /// <typeparam name="M"></typeparam>
        protected virtual void AddRequestMarker<M>() where M : IMarker
        {
            ToOwnerRequestMarkers.Add(new RequestMarkerHandler<M>((request, marker) =>
            {
                var component = GetComponent<IMessageRespounder>((T) marker.ObjKey);
                return component != null ? component.SendRequest(request) : null;
            }));
        }

        protected virtual void Subscribe()
        {
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
    }
}