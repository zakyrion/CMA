using CMA.Markers;
using CMA.Messages.Mediators;

namespace CMA.Messages
{
    public delegate void MessageMarkerDelegate<T, K>(T message, K marker) where T : IMessage where K : IMarker;

    public delegate void MessageDelegate<T>(T message) where T : IMessage;

    public delegate R RequestMarkerDelegate<R, T, K>(T request, K marker) where T : IRequest where K : IMarker;

    public delegate T RequestDelegate<T, K>(K message);

    public delegate T RequestSimpleDelegate<T>();

    public interface IMessageManager : IMessageRespounder
    {
        void AddRequestMarker(IRequestMarkerHandler handler);
        void RemoveRequestMarker(IRequestMarkerHandler handler);

        void AddMessageMarker(IMessageMarkerHandler handler);
        void RemoveMessageMarker(IMessageMarkerHandler handler);

        IMessageManager NewWithType();

        void SubscribeMessage(IMessageHandler handler);
        void SubscribeMessage<T>(MessageDelegate<T> @delegate) where T : IMessage;
        void SubscribeMessage<K, T>(MessageDelegate<T> @delegate) where T : IMessage where K : IMessage;
        void RemoveMessageReciever(IMessageHandler handler);
        void RemoveMessageReciever<T>() where T : IMessage;
        void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage;
        void RemoveMessageReciever<K, T>(MessageDelegate<T> @delegate) where T : IMessage where K : IMessage;

        void SubscribeRequest(IRequestHandler handler);
        void SubscribeRequest<T>(RequestSimpleDelegate<T> @delegate);
        void SubscribeRequest<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest;
        void RemoveRequestReciever(IRequestHandler handler);
        void RemoveRequestReciever<T>(RequestSimpleDelegate<T> @delegate);
        void RemoveRequestReciever<T, K>(RequestDelegate<T, K> @delegate) where K : IRequest;

        void SubscribeMediator(IMessageMediator mediator);
        void SubscribeMediator(IRequestMediator mediator);
        void RemoveMediator(IRequestMediator mediator);
        void RemoveMediator(IMessageMediator mediator);

        void Quit();
    }
}