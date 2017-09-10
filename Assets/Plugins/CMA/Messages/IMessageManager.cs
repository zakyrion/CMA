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

using System;
using CMA.Markers;

namespace CMA.Messages
{
    public delegate void MessageMarkerDelegate<T, K>(T message, K marker) where T : IMessage where K : IMarker;

    public interface IMessageManager : IMessageRespounder
    {
        int Id { get; }
        string TraceMarker { set; }
        IMessage Message { get; }
        void AddMarker(IMessageMarkerHandler handler);
        void RemoveMarker(IMessageMarkerHandler handler);

        IMessageManager NewWithType();

        //TODO:
        /// <summary>
        /// подписка теперь работает на любой тип, 
        /// IMessage превращаеться в системный тип передачи сообщений, который хранит относительный путь, данные, и обратный путь
        /// путь, может состоять из строки, или же цепочки маркеров. путь относительный, от какого либо актера,
        /// путь начинаеться если актер находит себя, в первой части пути.
        /// Imessage - Больше не хранит делегат на цепочку действия, действие остаеться у актера,
        /// каждое сообщение имеет свой Id, и если сообщение имеет действие, он отправляет в обратный путь системное сообщение Done
        /// сообщение можно отправить в 3 способа, synk, asynk и singleton.
        /// synk - получают все, но по очереди
        /// asynk - все кто получат, без лока
        /// singleton - по очереди, первый кто получит
        /// возможность создания цепочек событий которые блокируют друг-друга, если ожидают ответ
        /// в таком случае, если в очереди есть событие которое не блокируеться событием, которое ожидает ответ,
        /// начнет выполняться следующее неблокируемое событие, как только прийдет ответ, выполниться приостановленное событие
        /// добавить MonoActor - для работы с API unity, которые всегда работают только UpdatedMessageManager
        /// </summary>

        void Receive<T>(Action @delegate);
        void Receive<T>(Action<T> @delegate);
        void RemoveReceiver<T>(Action<T> @delegate);
        void Responce(IMessage message);
        void Transmit(IMessage message);
        void InvokeAtManager(Action action);

        void Quit();
    }
}