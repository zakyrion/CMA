using System.Collections.Generic;
using System.Linq;
using CMA.Markers;

namespace CMA.Messages
{
    public class MessageManager : IMessageManager
    {
        protected Dictionary<string, List<IMessageMarkerHandler>> MarkerMessageRecievers =
            new Dictionary<string, List<IMessageMarkerHandler>>();

        protected Dictionary<string, IRequestMarkerHandler> MarkerRequestRecievers =
            new Dictionary<string, IRequestMarkerHandler>();

        protected Dictionary<string, List<IMessageHandler>> MessageRecievers =
            new Dictionary<string, List<IMessageHandler>>();

        protected Dictionary<string, IRequestHandler> SimpleRequestRecievers = new Dictionary<string, IRequestHandler>();

        protected Dictionary<RequestKey, IRequestHandler> RequestRecievers =
            new Dictionary<RequestKey, IRequestHandler>();

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
                for (var i = 0; i < MessageRecievers[key].Count; i++)
                    MessageRecievers[key][i].Invoke(message);
            else
                foreach (var marker in message.Markers)
                    if (MarkerMessageRecievers.ContainsKey(marker.Key))
                        foreach (var handler in MarkerMessageRecievers[marker.Key])
                            handler.Invoke(message);
        }

        public bool ContainsMessage<T>() where T : IMessage
        {
            var key = typeof(T).Name;
            return MessageRecievers.ContainsKey(key);
        }

        public bool ContainsMessage(IMessage message)
        {
            var key = message.GetType().Name;
            return MessageRecievers.ContainsKey(key) ||
                   message.Markers.Any(marker => MarkerMessageRecievers.ContainsKey(marker.Key));
        }

        public void SubscribeMessageReciever(IMessageHandler handler)
        {
            var key = handler.Key;

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(handler);
        }

        public virtual void SubscribeMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            var key = typeof(T).Name;

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(new MessageHandler<T>(@delegate));
        }

        public virtual void SubscribeMessageReciever<K, T>(MessageDelegate<T> @delegate) where K : IMessage
            where T : IMessage
        {
            var key = typeof(K).Name;

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
            var key = typeof(T).Name;

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers.Remove(key);
        }

        public virtual void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            var key = typeof(T).Name;

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
        }

        public virtual void RemoveMessageReciever<K, T>(MessageDelegate<T> @delegate) where K : IMessage
            where T : IMessage
        {
            var key = typeof(K).Name;

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
        }

        public object SendRequest(IRequest request)
        {
            //сделать разделение реквестов с параметарми и без.
            //реквесты без параметров хранятся в своей коллекции.
            //реквесты с параметрами хранятся отдельно и проверяются на совпадение сообщения и результата

            foreach (var marker in request.Markers)
                if (MarkerRequestRecievers.ContainsKey(marker.Key))
                    return MarkerRequestRecievers[marker.Key].Invoke(request);

            if (request.RequestKey != null && RequestRecievers.ContainsKey(request.RequestKey.Value))
                return RequestRecievers[request.RequestKey.Value].Invoke(request);

            if (SimpleRequestRecievers.ContainsKey(request.ResultKey))
                return SimpleRequestRecievers[request.ResultKey].Invoke(request);

            return null;
        }

        #endregion

        #region Requests

        public virtual T SendRequest<T>(IRequest request)
        {
            if (typeof (T) != typeof(object))
                request.Initalize<T>();

            return (T)SendRequest(request);
        }

        public T SendRequest<T>()
        {
            return SendRequest<T>(new SimpleRequest<T>());
        }

        public bool ContainsRequest<T>()
        {
            var key = typeof(T).Name;
            return SimpleRequestRecievers.ContainsKey(key);
        }

        public bool ContainsRequest(IRequest request)
        {
            return request.Markers.Any(marker => MarkerRequestRecievers.ContainsKey(marker.Key)) || request.RequestKey != null && RequestRecievers.ContainsKey(request.RequestKey.Value) || SimpleRequestRecievers.ContainsKey(request.ResultKey);
        }

        public void SubscribeRequestReciever(IRequestHandler handler)
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

        public virtual void SubscribeRequestReciever<T>(RequestSimpleDelegate<T> @delegate)
        {
            var key = typeof(T).Name;
            SimpleRequestRecievers[key] = new RequestSimpleHandler<T>(@delegate);
        }

        public virtual void SubscribeRequestReciever<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest
        {
            var key = typeof(T).Name;
            var messageKey = typeof(K).Name;
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
            var key = typeof(T).Name;
            if (SimpleRequestRecievers.ContainsKey(key))
                SimpleRequestRecievers.Remove(key);
        }

        public virtual void RemoveRequestReciever<T, K>(RequestDelegate<T, K> @delegate)
            where K : IRequest
        {
            var key = typeof(T).Name;
            var messageKey = typeof(T).Name;

            var requsetKey = new RequestKey(key, messageKey);
            if (RequestRecievers.ContainsKey(requsetKey))
                RequestRecievers.Remove(requsetKey);
        }

        #endregion
    }
}