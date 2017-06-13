//   Copyright {CMA} {Kharsun Sergei}
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
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
        int Id { get; }
        string TraceMarker { set; }
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