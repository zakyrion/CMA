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
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CMA.Messages
{
    public class ThreadMessageManager : MessageManager
    {
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        protected Queue<Action> Actions = new Queue<Action>();
        protected Thread Thread;

        public ThreadMessageManager()
        {
            Thread = new Thread(Update);
            Thread.Start();
        }

        public override IMessageManager NewWithType()
        {
            return new ThreadMessageManager();
        }

        public override void InvokeAtManager(Action action)
        {
            lock (Lock)
            {
                Actions.Enqueue(action);
                _event.Set();
            }
        }

        public override void SendMessage(IMessage message)
        {
            lock (Lock)
            {
                Actions.Enqueue(() => { base.SendMessage(message); });
                _event.Set();
            }
        }

        protected virtual void Update()
        {
            while (true)
            {
                Action[] actions = null;

                do
                {
                    if (Actions.Count > 0)
                    {
                        actions = Actions.ToArray();
                        Actions.Clear();
                    }

                    if (Actions == null)
                        _event.WaitOne();
                } while (Actions == null);

                if (actions != null)
                    foreach (var action in actions)
                        action();
            }
        }

        public override void Quit()
        {
            Debug.Log("Quit Manager");
            Thread.Abort();
            base.Quit();
        }
    }
}