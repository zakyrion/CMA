using System;
using CMA.Markers;

namespace CMA.Messages
{
    public delegate void MessageDelegate<T>(T message) where T : IMessage;

    public delegate T RequestDelegate<T,K>(K message);

    public delegate T RequestSimpleDelegate<T>();

    public interface IMessageManager
    {
        void AddRequestMarker(IRequestMarkerHandler handler);
        void RemoveRequestMarker(IRequestMarkerHandler handler);

        void AddMessageMarker(IMessageMarkerHandler handler);
        void RemoveMessageMarker(IMessageMarkerHandler handler);

        IMessageManager NewWithType();
        void SendMessage(IMessage message);
        bool ContainsMessage<T>() where T: IMessage;
        bool ContainsMessage(IMessage message);
        void SubscribeMessageReciever(IMessageHandler handler);
        void SubscribeMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage;
        void SubscribeMessageReciever<K, T>(MessageDelegate<T> @delegate) where T : IMessage where K : IMessage;
        void RemoveMessageReciever(IMessageHandler handler);
        void RemoveMessageReciever<T>() where T : IMessage;
        void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage;
        void RemoveMessageReciever<K, T>(MessageDelegate<T> @delegate) where T : IMessage where K : IMessage;

        object SendRequest(IRequest request);
        T SendRequest<T>(IRequest request);
        T SendRequest<T>();
        bool ContainsRequest<T>();
        bool ContainsRequest(IRequest request);
        void SubscribeRequestReciever(IRequestHandler handler);
        void SubscribeRequestReciever<T>(RequestSimpleDelegate<T> @delegate);
        void SubscribeRequestReciever<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest;
        void RemoveRequestReciever(IRequestHandler handler);
        void RemoveRequestReciever<T>(RequestSimpleDelegate<T> @delegate);
        void RemoveRequestReciever<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest;
    }
}