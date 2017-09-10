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

namespace CMA.Messages
{
    public class ThreadPoolMessageManager : MessageManager
    {
        AutoResetEvent _event = new AutoResetEvent(true);
        private bool _isAtCicle;
        protected Queue<Action> Actions = new Queue<Action>();

        public override IMessageManager NewWithType()
        {
            return new ThreadPoolMessageManager();
        }

        public override void InvokeAtManager(Action action)
        {
            _event.WaitOne();

            Actions.Enqueue(action);

            if (!_isAtCicle)
            {
                _isAtCicle = true;
                ThreadPool.QueueUserWorkItem(state => { Tick(); });
            }

            _event.Set();
        }

        public override void Responce(IMessage message)
        {
            _event.WaitOne();

            Actions.Enqueue(() => { base.Responce(message); });

            if (!_isAtCicle)
            {
                _isAtCicle = true;
                ThreadPool.QueueUserWorkItem(state => { Tick(); });
            }

            _event.Set();
        }

        public override void Transmit(IMessage message)
        {
            _event.WaitOne();

            Actions.Enqueue(() => { base.Transmit(message); });

            if (!_isAtCicle)
            {
                _isAtCicle = true;
                ThreadPool.QueueUserWorkItem(state => { Tick(); });
            }

            _event.Set();
        }

        protected virtual void Tick()
        {
            _event.WaitOne();

            Action action = null;
            if (Actions.Count > 0)
                action = Actions.Dequeue();
            else
                _isAtCicle = false;

            _event.Set();

            if (action != null)
            {
                base.InvokeAtManager(action);
                ThreadPool.QueueUserWorkItem(state => { Tick(); });
            }
        }
    }
}