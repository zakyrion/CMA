using System.Collections.Generic;
using System.Linq;
using CMA.Markers;
using CMA.Messages.Mediators;

namespace CMA.Messages
{
    public class MessageManager : IMessageManager
    {
        protected Dictionary<string, List<IMessageMarkerHandler>> MarkerMessageRecievers =
            new Dictionary<string, List<IMessageMarkerHandler>>();

        protected Dictionary<string, IRequestMarkerHandler> MarkerRequestRecievers =
            new Dictionary<string, IRequestMarkerHandler>();

        protected Dictionary<string, List<IMessageMediator>> MessageMediators =
            new Dictionary<string, List<IMessageMediator>>();

        protected Dictionary<string, List<IMessageHandler>> MessageRecievers =
            new Dictionary<string, List<IMessageHandler>>();

        protected Dictionary<RequestKey, IRequestMediator> RequestMediators =
            new Dictionary<RequestKey, IRequestMediator>();

        protected Dictionary<RequestKey, IRequestHandler> RequestRecievers =
            new Dictionary<RequestKey, IRequestHandler>();

        protected Dictionary<string, IRequestMediator> SimpleRequestMediators =
            new Dictionary<string, IRequestMediator>();

        protected Dictionary<string, IRequestHandler> SimpleRequestRecievers = new Dictionary<string, IRequestHandler>();

        #region Messages

        public void AddRequestMarker(IRequestMarkerHandler handler)
        {
            var key = handler.Key;
            MarkerRequestRecievers[key] = handler;
        }

        public void RemoveRequestMarker(IRequestMarkerHandler handler)
        {
            var key = handler.Key;

            if (MarkerRequestRecievers.ContainsKey(key) && MarkerRequestRecievers[key].Equals(handler))
                MarkerRequestRecievers.Remove(key);
        }

        public void AddMessageMarker(IMessageMarkerHandler handler)
        {
            var key = handler.Key;

            if (!MarkerMessageRecievers.ContainsKey(key))
                MarkerMessageRecievers.Add(key, new List<IMessageMarkerHandler>());

            MarkerMessageRecievers[key].Add(handler);
        }

        public void RemoveMessageMarker(IMessageMarkerHandler handler)
        {
            var key = handler.Key;

            if (MarkerMessageRecievers.ContainsKey(key))
            {
                var handlers =
                    MarkerMessageRecievers[key].Where(messageHandler => messageHandler.Equals(handler)).ToList();

                foreach (var messageHandler in handlers)
                    MarkerMessageRecievers[key].Remove(messageHandler);
            }
        }

        public virtual IMessageManager NewWithType()
        {
            return new MessageManager();
        }

        public virtual void SendMessage(IMessage message)
        {
            var key = message.GetType().Name;

            if (MessageRecievers.ContainsKey(key))
            {
                for (var i = 0; i < MessageRecievers[key].Count; i++)
                    MessageRecievers[key][i].Invoke(message);
                return;
            }

            if (MessageMediators.ContainsKey(key))
            {
                foreach (var messageMediator in MessageMediators[key])
                    messageMediator.TransmitMessage(message);
                return;
            }

            foreach (var marker in message.Markers)
                if (MarkerMessageRecievers.ContainsKey(marker.MarkerKey))
                {
                    foreach (var handler in MarkerMessageRecievers[marker.MarkerKey])
                        handler.Invoke(message);
                    return;
                }

            message.Fail();
        }

        public bool ContainsMessage<T>() where T : IMessage
        {
            var key = typeof (T).Name;
            return MessageRecievers.ContainsKey(key) || MessageMediators.ContainsKey(key);
        }

        public bool ContainsMessage(IMessage message)
        {
            var key = message.GetType().Name;
            return MessageRecievers.ContainsKey(key) ||
                   message.Markers.Any(marker => MarkerMessageRecievers.ContainsKey(marker.MarkerKey)) ||
                   MessageMediators.ContainsKey(key);
        }

        public void SubscribeMessage(IMessageHandler handler)
        {
            var key = handler.Key;

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(handler);
        }

        public virtual void SubscribeMessage<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            var key = typeof (T).Name;

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(new MessageHandler<T>(@delegate));
        }

        public virtual void SubscribeMessage<K, T>(MessageDelegate<T> @delegate) where K : IMessage
            where T : IMessage
        {
            var key = typeof (K).Name;

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(new MessageHandler<T>(@delegate));
        }

        public void RemoveMessageReciever(IMessageHandler handler)
        {
            var key = handler.Key;

            if (MessageRecievers.ContainsKey(key))
            {
                var handlers =
                    MessageRecievers[key].Where(messageHandler => messageHandler.Equals(handler)).ToList();

                foreach (var messageHandler in handlers)
                    MessageRecievers[key].Remove(messageHandler);
            }
        }

        public virtual void RemoveMessageReciever<T>() where T : IMessage
        {
            var key = typeof (T).Name;

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers.Remove(key);
        }

        public virtual void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            var key = typeof (T).Name;

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
        }

        public virtual void RemoveMessageReciever<K, T>(MessageDelegate<T> @delegate) where K : IMessage
            where T : IMessage
        {
            var key = typeof (K).Name;

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
        }

        public virtual object SendRequest(IRequest request)
        {
            request.Initalize();
            TransmitRequest(request);
            return request.Result;
        }

        protected virtual void TransmitRequest(IRequest request)
        {
            foreach (var marker in request.Markers)
                if (MarkerRequestRecievers.ContainsKey(marker.MarkerKey))
                {
                    MarkerRequestRecievers[marker.MarkerKey].Invoke(request);
                    return;
                }

            if (request.RequestKey != null && RequestRecievers.ContainsKey(request.RequestKey.Value))
            {
                RequestRecievers[request.RequestKey.Value].Invoke(request);
                return;
            }

            if (SimpleRequestRecievers.ContainsKey(request.ResultKey))
            {
                SimpleRequestRecievers[request.ResultKey].Invoke(request);
                return;
            }

            request.Fail();
        }

        #endregion

        #region Requests

        public virtual T SendRequest<T>(IRequest request)
        {
            SendRequest(request);
            return (T) request.Result;
        }

        public virtual T SendRequest<T>()
        {
            return SendRequest<T>(new SimpleRequest<T>());
        }

        public bool ContainsRequest<T>()
        {
            var key = typeof (T).Name;
            return SimpleRequestRecievers.ContainsKey(key) || SimpleRequestMediators.ContainsKey(key);
        }

        public bool ContainsRequest(IRequest request)
        {
            return request.Markers.Any(marker => MarkerRequestRecievers.ContainsKey(marker.MarkerKey))
                   ||
                   (request.RequestKey != null &&
                    (RequestRecievers.ContainsKey(request.RequestKey.Value) ||
                     RequestMediators.ContainsKey(request.RequestKey.Value))) ||
                   SimpleRequestRecievers.ContainsKey(request.ResultKey) ||
                   SimpleRequestMediators.ContainsKey(request.ResultKey);
        }

        public void SubscribeRequest(IRequestHandler handler)
        {
            if (handler.MessageKey != null)
            {
                var requestKey = new RequestKey(handler.Key, handler.MessageKey);
                RequestRecievers[requestKey] = handler;
            }
            else
            {
                var key = handler.Key;
                SimpleRequestRecievers[key] = handler;
            }
        }

        public virtual void SubscribeRequest<T>(RequestSimpleDelegate<T> @delegate)
        {
            var key = typeof (T).Name;
            SimpleRequestRecievers[key] = new RequestSimpleHandler<T>(@delegate);
        }

        public virtual void SubscribeRequest<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest
        {
            var key = typeof (T).Name;
            var messageKey = typeof (K).Name;
            var requestKey = new RequestKey(key, messageKey);
            RequestRecievers[requestKey] = new RequestHandler<T, K>(@delegate);
        }

        public virtual void RemoveRequestReciever(IRequestHandler handler)
        {
            var key = handler.Key;
            var messageKey = handler.MessageKey;
            var requsetKey = new RequestKey(key, messageKey);

            if (messageKey == null && SimpleRequestRecievers.ContainsKey(key))
                SimpleRequestRecievers.Remove(key);
            else if (messageKey != null && RequestRecievers.ContainsKey(requsetKey) &&
                     RequestRecievers[requsetKey].Equals(handler))
                RequestRecievers.Remove(requsetKey);
        }

        public virtual void RemoveRequestReciever<T>(RequestSimpleDelegate<T> @delegate)
        {
            var key = typeof (T).Name;
            if (SimpleRequestRecievers.ContainsKey(key))
                SimpleRequestRecievers.Remove(key);
        }

        public virtual void RemoveRequestReciever<T, K>(RequestDelegate<T, K> @delegate)
            where K : IRequest
        {
            var key = typeof (T).Name;
            var messageKey = typeof (T).Name;

            var requsetKey = new RequestKey(key, messageKey);
            if (RequestRecievers.ContainsKey(requsetKey))
                RequestRecievers.Remove(requsetKey);
        }

        public void SubscribeMediator(IMessageMediator mediator)
        {
            var key = mediator.Key;

            if (MessageMediators.ContainsKey(key))
                MessageMediators[key].Remove(mediator);
        }

        public void SubscribeMediator(IRequestMediator mediator)
        {
            if (mediator.RequestKey == null)
                SimpleRequestMediators[mediator.ResultKey] = mediator;
            else
                RequestMediators[mediator.RequestKey.Value] = mediator;
        }

        public void RemoveMediator(IRequestMediator mediator)
        {
            if (mediator.RequestKey == null)
                SimpleRequestMediators.Remove(mediator.ResultKey);
            else
                RequestMediators.Remove(mediator.RequestKey.Value);
        }

        public void RemoveMediator(IMessageMediator mediator)
        {
            var key = mediator.Key;

            if (!MessageMediators.ContainsKey(key))
                MessageMediators.Add(key, new List<IMessageMediator>());

            MessageMediators[key].Add(mediator);
        }

        #endregion
    }
}