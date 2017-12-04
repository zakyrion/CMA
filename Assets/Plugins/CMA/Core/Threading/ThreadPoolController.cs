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
using System.Threading.Tasks;
using CMA.Messages;

namespace CMA.Core
{
    public class ThreadPoolController : ThreadController
    {
        protected object Lock = new object();
        protected Task Task;

        public override void Invoke(Action action)
        {
            lock (Lock)
            {
                if (Task == null)
                {
                    Task = new Task(action);
                    Task.Start();
                }
                else
                {
                    Task = Task.ContinueWith(task => action);
                }
            }
        }

        public override void Invoke<T>(Action<T> action, T param)
        {
            lock (Lock)
            {
                var handler = new SimpleActionHandler<T>(action, param);
                if (Task == null)
                {
                    Task = new Task(handler.Invoke);
                    Task.Start();
                }
                else
                {
                    Task = Task.ContinueWith(task => handler.Invoke());
                }
            }
        }
    }
}