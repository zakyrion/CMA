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
using System.Collections.Concurrent;
using System.Threading;
using CMA.Messages;

namespace CMA.Core
{
    public class ThreadPoolController : ThreadController
    {
        protected object Lock = new object();
        private bool _isAtCicle;
        protected ConcurrentQueue<Action> Actions = new ConcurrentQueue<Action>();

        public override void Invoke(Action action)
        {
            Actions.Enqueue(action);

            lock (Lock)
            {
                if (!_isAtCicle)
                {
                    _isAtCicle = true;
                    ThreadPool.QueueUserWorkItem(state => { Tick(); });
                }
            }
        }

        public override void Invoke<T>(Action<T> action, T param)
        {
            var handler = new SimpleActionHandler<T>(action, param);
            Actions.Enqueue(handler.Invoke);
            
            lock (Lock)
            {
                if (!_isAtCicle)
                {
                    _isAtCicle = true;
                    ThreadPool.QueueUserWorkItem(state => { Tick(); });
                }
            }
        }

        protected virtual void Tick()
        {
            Action action = null;

            while (Actions.TryDequeue(out action))
                action();

            lock (Lock)
            {
                _isAtCicle = false;
            }
        }
    }
}