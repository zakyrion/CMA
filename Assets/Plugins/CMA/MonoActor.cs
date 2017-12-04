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

        public IThreadController ThreadController => Actor.ThreadController;
        public string Adress => Actor.Adress;
        public ICluster Cluster => Actor.Cluster;


        public void OnAdd(ICluster cluster, string adress)
        {
            Actor.OnAdd(cluster, adress);
        }

        public void Quit()
        {
            if (!_isQuit)
            {
                _isQuit = true;
                Destroy(gameObject);
            }
        }

        public void Send(object data, string adress, string cluster = "")
        {
            Actor.Send(data, adress, cluster);
        }

        public void Send(object data, Action action, string adress, string cluster = "")
        {
            Actor.Send(data, action, adress, cluster);
        }

        public void Ask<TR>(Action<TR> action, string adress, string cluster = "")
        {
            Actor.Ask(action, adress, cluster);
        }

        public void Ask<TM, TR>(TM data, Action<TR> action, string adress, string cluster = "")
        {
            Actor.Ask(data, action, adress, cluster);
        }

        public void PushMessage(IMessage message)
        {
            Actor.PushMessage(message);
        }

        public void Respounce(IMessage message, object data = null)
        {
            Actor.Respounce(message, data);
        }

        public void Quit(bool needKillBaseActor)
        {
            if (!_isQuit)
            {
                _isQuit = true;
                if (needKillBaseActor)
                    Actor.Quit();
            }
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
            Receive<Kill>(OnKill);
            Subscribe();
        }

        private void OnKill(IMessage obj)
        {
            Actor.ThreadController.Invoke(OnKillHandler);
        }

        private void OnKillHandler()
        {
            Quit();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            Quit(true);
        }

        #endregion
    }
}