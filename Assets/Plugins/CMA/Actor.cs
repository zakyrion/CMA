﻿//   Copyright {CMA} {Kharsun Sergei}
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
using System.Collections.Generic;
using System.Threading;
using CMA.Markers;
using CMA.Messages;

namespace CMA
{
    public static class ActorId
    {
        private static long _currentId = long.MinValue;

        public static long CurrentId
        {
            get { return Interlocked.Increment(ref _currentId); }
        }
    }

    public class Actor<K> : IKeyActor<K>
    {
        protected Dictionary<long, IActor> Cache = new Dictionary<long, IActor>();

        protected List<IActor> Childs = new List<IActor>();
        protected Dictionary<string, IKeyStorage> KeyCache = new Dictionary<string, IKeyStorage>();
        protected object Lock = new object();

        public Actor(K key)
        {
            Id = ActorId.CurrentId;
            Key = key;
            Manager = new ThreadPoolMessageManager();
            Manager.TraceMarker = GetType().ToString();
            KeyType = typeof(K).ToString();

            Subscribe();
        }

        public Actor(K key, IMessageManager manager)
        {
            Id = ActorId.CurrentId;
            Key = key;
            Manager = manager;
            Manager.TraceMarker = GetType().ToString();
            KeyType = typeof(K).ToString();

            Subscribe();
        }

        public object CustomKey { get; protected set; }

        public IMessageManager Manager { get; protected set; }
        public IMarker Marker { get; protected set; }
        public long Id { get; protected set; }
        public IActor Owner { get; protected set; }
        public object Key { get; protected set; }
        public string KeyType { get; protected set; }
        protected IMessage Message { get { return Manager.Message; } }

        public virtual void OnAdd(IActor owner)
        {
            if (Owner == null)
                Owner = owner;
        }

        public virtual void OnRemove()
        {
        }

        public virtual void Quit()
        {
            Manager.Quit();
            foreach (var child in Childs)
                child.Quit();
        }

        public virtual bool Contains(long id)
        {
            lock (Lock)
            {
                return Cache.ContainsKey(id);
            }
        }

        public virtual bool Contains<T>(T id)
        {
            lock (Lock)
            {
                var type = typeof(T).ToString();
                return KeyCache.ContainsKey(type) && KeyCache[type].Contain(id);
            }
        }

        public bool Contains(object key)
        {
            lock (Lock)
            {
                var type = key.GetType().ToString();
                return KeyCache.ContainsKey(type) && KeyCache[type].Contain(key);
            }
        }

        public bool Contains(IActor actor)
        {
            lock (Lock)
            {
                return Childs.Contains(actor);
            }
        }

        public virtual void AddActor<T>(IKeyActor<T> actor)
        {
            lock (Lock)
            {
                Childs.Add(actor);
                Cache.Add(actor.Id, actor);
                actor.OnAdd(this);

                var type = typeof(T).ToString();

                if (!KeyCache.ContainsKey(type))
                {
                    var storage = new KeyStorage<T>();
                    storage.AddKVP(actor.Key, actor);
                    KeyCache.Add(type, storage);
                }
                else
                {
                    KeyCache[type].AddKVP(actor.Key, actor);
                }
            }
        }

        public void RemoveActor(long id)
        {
            lock (Lock)
            {
                var actor = Cache[id];
                KeyCache[actor.KeyType].Remove(actor.Key);
                Childs.Remove(actor);
                Cache.Remove(id);
                actor.OnRemove();
            }
        }

        public void RemoveActor(IActor actor)
        {
            lock (Lock)
            {
                RemoveActor(actor.Id);
            }
        }

        public void RemoveActor<T>(T key)
        {
            lock (Lock)
            {
                var keyType = typeof(T).ToString();
                var actor = KeyCache[keyType].Get<IActor>(key);
                KeyCache[keyType].Remove(key);
                Childs.Remove(actor);
                Cache.Remove(actor.Id);
                actor.OnRemove();
            }
        }

        public T GetActor<T>(long id)
        {
            lock (Lock)
            {
                return (T)Cache[id];
            }
        }

        public IActor GetActor<T>(T key)
        {
            lock (Lock)
            {
                var result = default(IActor);
                var type = typeof(T).ToString();

                if (KeyCache.ContainsKey(type) && KeyCache[type].Contain(key))
                    return KeyCache[type].Get<IActor>(key);

                return result;
            }
        }

        public R GetActor<R, K1>(K1 key)
        {
            lock (Lock)
            {
                var result = default(R);
                var type = typeof(K1).ToString();

                if (KeyCache.ContainsKey(type) && KeyCache[type].Contain(key))
                    return KeyCache[type].Get<R>(key);

                return result;
            }
        }

        public T GetActor<T>()
        {
            lock (Lock)
            {
                foreach (var child in Childs)
                    if (child is T)
                        return (T)child;
            }

            return default(T);
        }

        public void Send(IMessage message)
        {
            if (message.MessageManager == null)
                message.MessageManager = Manager;

            Manager.InvokeAtManager(() => { SendMessageHide(message); });
        }

        public virtual void Send(object data, IActionHandler action = null)
        {
            var message = new Message(data) { MessageManager = Manager };
            Manager.InvokeAtManager(() => { SendMessageHide(message); });
        }

        public virtual void Send(object data, params IMarker[] markers)
        {
            var message = new Message(data) { MessageManager = Manager };
            message.AddMarkers(markers);
            Manager.InvokeAtManager(() => { SendMessageHide(message); });
        }

        public virtual void Send(object data, IActionHandler action, params IMarker[] markers)
        {
            var message = new Message(data, action) { MessageManager = Manager };
            message.AddMarkers(markers);
            Manager.InvokeAtManager(() => { SendMessageHide(message); });
        }

        public void InvokeAt(Action action)
        {
            Manager.InvokeAtManager(action);
        }

        public K TypedKey
        {
            get { return (K)Key; }
        }

        protected void SendMessageHide(IMessage message)
        {
            if (!message.IsDone)
            {
                if (message.IsContainsActorId(Id))
                    return;

                message.AddActorId(Id);
                message.AddTrace(GetType().ToString());

                if (Manager.CanRespond(message))
                {
                    Manager.Responce(message);
                    return;
                }

                if (Manager.CanTransmit(message))
                {
                    Manager.Transmit(message);
                    return;
                }

                var isSended = false;

                foreach (var child in Childs)
                    if (child.CanRespond(message))
                    {
                        child.Send(message);
                        isSended = true;
                    }

                if (isSended)
                    return;

                foreach (var child in Childs)
                    if (child.CanTransmit(message))
                    {
                        child.Send(message);
                        isSended = true;
                    }

                if (isSended)
                {
                    if (Marker != null)
                        message.AddMarkerForReturn(Marker);

                    return;
                }

                if (Owner != null)
                {
                    if (Marker != null)
                        message.AddMarkerForReturn(Marker);
                    Owner.Send(message);
                }
            }
        }

        protected virtual void Subscribe()
        {
            
        }

        public virtual void Receive<T>(Action @delegate)
        {
            Manager.Receive<T>(@delegate);
        }

        public virtual void Receive<T>(Action<T> @delegate)
        {
            Manager.Receive(@delegate);
        }

        protected void AddMarker<M>() where M : IMarker
        {
            var handler = new MessageMarkerHandler<M>((message, marker) =>
            {
                if (Contains(marker.ObjKey))
                {
                    marker.Check();
                    KeyCache[marker.ObjKeyType].Get<IActor>(marker.ObjKey).Send(message);
                }
            });
            Manager.AddMarker(handler);
        }

        protected void AddMarker<M>(MessageMarkerDelegate<IMessage, M> @delegate) where M : IMarker
        {
            var handler = new MessageMarkerHandler<M>(@delegate);
            Manager.AddMarker(handler);
        }

        protected void RemoveMarker(IMessageMarkerHandler handler)
        {
            Manager.RemoveMarker(handler);
        }

        public bool CanRespond(IMessage message)
        {
            return Manager.CanRespond(message);
        }

        public bool CanTransmit(IMessage message)
        {
            return Manager.CanTransmit(message);
        }
    }
}