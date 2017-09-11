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
using UnityEngine;

namespace CMA
{
    public abstract class MonoActor<T> : MonoBehaviour, IKeyActor<T>
    {
        protected Actor<T> Actor;

        protected IMessage Message
        {
            get { return Actor.Manager.Message; }
        }

        public bool CanRespond(IMessage message)
        {
            return Actor.CanRespond(message);
        }

        public bool CanTransmit(IMessage message)
        {
            return Actor.CanTransmit(message);
        }

        public IMessageManager Manager
        {
            get { return Actor.Manager; }
        }

        public IMarker Marker { get; protected set; }

        public long Id
        {
            get { return Actor.Id; }
        }

        public IActor Owner
        {
            get { return Actor.Owner; }
        }

        public object Key
        {
            get { return Actor.Key; }
        }

        public string KeyType
        {
            get { return Actor.KeyType; }
        }

        public void OnAdd(IActor owner)
        {
            Actor.OnAdd(owner);
        }

        public virtual void OnRemove()
        {
            Actor.OnRemove();
        }

        public virtual void Quit()
        {
            Actor.Quit();
        }

        public bool Contains(long id)
        {
            return Actor.Contains(id);
        }

        public bool Contains<T1>(T1 id)
        {
            return Actor.Contains(id);
        }

        public bool Contains(object key)
        {
            return Actor.Contains(key);
        }

        public bool Contains(IActor actor)
        {
            return Actor.Contains(actor);
        }

        public void AddActor<T1>(IKeyActor<T1> actor)
        {
            Actor.AddActor(actor);
        }

        public void RemoveActor(long id)
        {
            Actor.RemoveActor(id);
        }

        public void RemoveActor(IActor actor)
        {
            Actor.RemoveActor(Actor);
        }

        public void RemoveActor<T1>(T1 key)
        {
            Actor.RemoveActor(key);
        }

        public T1 GetActor<T1>(long id)
        {
            return Actor.GetActor<T1>(id);
        }

        public IActor GetActor<T1>(T1 key)
        {
            return Actor.GetActor(key);
        }

        public R GetActor<R, K>(K key)
        {
            return Actor.GetActor<R, K>(key);
        }

        public T1 GetActor<T1>()
        {
            return Actor.GetActor<T1>();
        }

        public void Send(IMessage message)
        {
            Actor.Send(message);
        }

        public void Send(object data, IActionHandler action = null)
        {
            Actor.Send(data, action);
        }

        public void Send(object data, params IMarker[] markers)
        {
            Actor.Send(data, markers);
        }

        public void Send(object data, IActionHandler action, params IMarker[] markers)
        {
            Actor.Send(data, action, markers);
        }

        public void InvokeAt(Action action)
        {
            Actor.InvokeAt(action);
        }

        public T TypedKey
        {
            get { return Actor.TypedKey; }
        }

        public virtual IActor Init(T key)
        {
            Actor = new Actor<T>(key, new UpdatedMessageManager());
            Subscribe();
            return this;
        }

        protected abstract void Subscribe();

        public virtual void Receive<T1>(Action @delegate)
        {
            Manager.Receive<T1>(@delegate);
        }

        public virtual void Receive<T1>(Action<T1> @delegate)
        {
            Manager.Receive(@delegate);
        }

        #region Unity

        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
            if (Owner != null && Owner.Contains(TypedKey))
                Owner.RemoveActor(TypedKey);
        }

        #endregion
    }
}