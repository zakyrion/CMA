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
using CMA.Core;
using CMA.Messages;
using UnityEngine;

namespace CMA
{
    public abstract class MonoActor : MonoBehaviour, IActor
    {
        private bool _isQuit;
        protected Actor Actor;

        public string Adress => Actor.Adress;

        public IMailBox MailBox => Actor.MailBox;

        public void CheckMailBox()
        {
            Actor.CheckMailBox();
        }

        public void OnAdd(IMailBox mailBox, Func<IMessage[]> messagesRequest)
        {
            Actor.OnAdd(mailBox, messagesRequest);
        }

        public void Quit()
        {
            if (!_isQuit)
            {
                _isQuit = true;
                Actor.Quit();
                Destroy(gameObject);
            }
        }

        public void Send(object data, string adress = "")
        {
            Actor.Send(data, adress);
        }

        public void Send(object data, Action action, string adress = "")
        {
            Actor.Send(data, action, adress);
        }

        public void Ask<T>(Action<T> action, string adress = "")
        {
            Actor.Ask(action, adress);
        }

        public void Ask<TM, TR>(TM data, Action<TR> action, string adress = "")
        {
            Actor.Ask(data, action, adress);
        }

        public void Respounce(IMessage message, object data = null)
        {
            Actor.Respounce(message, data);
        }

        protected abstract void Subscribe();

        public virtual void Receive<T>(Action<IMessage> @delegate)
        {
            Actor.Receive<T>(@delegate);
        }

        public virtual void Receive<T>(Action<T, IMessage> @delegate)
        {
            Actor.Receive(@delegate);
        }


        #region Unity

        protected virtual void Awake()
        {
            Actor = new Actor(new MainThreadController());
            Subscribe();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            Quit();
        }

        #endregion
    }
}