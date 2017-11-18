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
    public class SingleThreadController : ThreadController
    {
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        protected ConcurrentQueue<Action> Actions = new ConcurrentQueue<Action>();
        protected Thread Thread;

        public SingleThreadController()
        {
            Thread = new Thread(Update);
            Thread.Start();
        }

        public override void Invoke(Action action)
        {
            Actions.Enqueue(action);

            _event.Set();
        }

        public override void Invoke<T>(Action<T> action, T param)
        {
            var handler = new SimpleActionHandler<T>(action, param);
            Actions.Enqueue(handler.Invoke);

            _event.Set();
        }

        protected virtual void Update()
        {
            while (true)
            {
                if (Actions.Count == 0)
                    _event.WaitOne();

                Action action = null;

                while (Actions.TryDequeue(out action))
                    action();
            }
        }

        public override void Remove()
        {
            Thread.Abort();
        }
    }
}