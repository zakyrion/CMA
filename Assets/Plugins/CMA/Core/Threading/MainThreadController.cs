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
using CMA.Messages;
using UnityEngine;

namespace CMA.Core
{
    public class MainThreadController : ThreadController, IUpdated
    {
        protected Queue<Action> Actions = new Queue<Action>();

        public MainThreadController()
        {
            UpdateService.Instance.Add(this);
        }

        public void Update()
        {
            try
            {
                Action[] actions = null;

                lock (Lock)
                {
                    if (Actions.Count > 0)
                    {
                        actions = Actions.ToArray();
                        Actions.Clear();
                    }
                }

                if (actions != null)
                    foreach (var action in actions)
                        action();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public override void Invoke(Action action)
        {
            lock (Lock)
            {
                Actions.Enqueue(action);
            }
        }

        public override void Invoke<T>(Action<T> action, T param)
        {
            lock (Lock)
            {
                var handler = new SimpleActionHandler<T>(action, param);
                Actions.Enqueue(handler.Invoke);
            }
        }

        public override void Remove()
        {
            UpdateService.Instance.Remove(this);
        }
    }
}