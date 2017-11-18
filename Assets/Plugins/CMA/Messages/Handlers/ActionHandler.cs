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
    public class ActionHandler<T> : IActionHandler
    {
        private readonly Action<T> _actionParams;

        public ActionHandler(Action<T> action)
        {
            _actionParams = action;
        }

        public void Invoke(object obj = null)
        {
            try
            {
                if (_actionParams != null)
                {
                    /*if (obj == null)
                        Debug.Log("Obj Null");*/

                    _actionParams((T) obj);
                }
            }
            catch (Exception e)
            {
                //Debug.Log($"Try cast to:{typeof(T)} but Type is:{obj.GetType()}");
            }
        }
    }

    public class ActionHandler : IActionHandler
    {
        private readonly Action _action;

        public ActionHandler(Action action)
        {
            _action = action;
        }

        public void Invoke(object obj = null)
        {
            try
            {
                _action();
            }
            catch (Exception e)
            {
                //Debug.Log(e);
            }
        }
    }
}