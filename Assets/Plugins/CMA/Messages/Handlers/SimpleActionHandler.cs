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
using UnityEngine;

namespace CMA.Messages
{
    public class SimpleActionHandler<T> : ISimpleActionHandler
    {
        private readonly Action<T> _action;
        private readonly object _lock = new object();
        private readonly T _param;

        public SimpleActionHandler(Action<T> action, T param)
        {
            _action = action;
            _param = param;
        }

        public void Invoke()
        {
            lock (_lock)
            {
                try
                {
                    _action?.Invoke(_param);

                }
                catch (Exception e)
                {
                    Debug.Log($"SimpleActionHandler exception: {e}");
                }
            }
        }
    }
}