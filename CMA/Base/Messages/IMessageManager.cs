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
    public delegate void MessageDelegate<T>(T message);

    public interface IMessageManager : IMessageRespounder
    {
        int Id { get; }
        string TraceMarker { set; }

        void AddMarker(IMessageMarkerHandler handler);
        void RemoveMarker(IMessageMarkerHandler handler);

        IMessageManager NewWithType();

        //TODO оставить только один тип подписки.
        //оставить возможность составлять цепочки из действий
        //не различать сообщения и запросы
        //оставить возможность подписки без типизации события, в этом случае расценивать его как простой запрос.
        //оставить маркеры.
        //убрать медиаторы, проверка на возможность получения у детей.
        //создать универсальный актер, для всех сущностей. 

        void SubscribeMessage<T>(MessageDelegate<T> @delegate) where T : IMessage;
        void SubscribeMessage<T>(MessageDelegate<IMessage> @delegate);
        void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage;

        void InvokeAtManager(Action action);

        void Quit();
    }
}