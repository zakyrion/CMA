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
using CMA.Messages;

namespace CMA
{
    public interface IActor
    {
        IMessageManager Manager { get; }
        IMarker Marker { get; }
        long Id { get; }
        IActor Owner { get; }
        object Key { get; }
        string KeyType { get; }

        void OnAdd(IActor owner);
        void OnRemove();
        void Quit();

        bool Contains(long id);
        bool Contains<T>(T id);
        bool Contains(object key);
        bool Contains(IActor actor);

        void AddActor<T>(IKeyActor<T> actor);

        void RemoveActor(long id);
        void RemoveActor(IActor actor);
        void RemoveActor<T>(T key);

        T GetActor<T>(long id);
        IActor GetActor<T>(T key);
        R GetActor<R, K>(K key);
        T GetActor<T>();

        void SendMessage(IMessage message);

        bool ContainsMessage<T>() where T : IMessage;
        bool ContainsMessage(IMessage message);

        void InvokeAt(Action action);
    }

    public interface IKeyActor<T> : IActor
    {
        T TypedKey { get; }
    }
}