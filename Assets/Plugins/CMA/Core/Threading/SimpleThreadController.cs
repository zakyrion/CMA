﻿//   Copyright {CMA} {Kharsun Sergei}
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

namespace CMA.Core
{
    public class SimpleThreadController : ThreadController
    {
        protected object Lock = new object();

        public override void Invoke(Action action)
        {
            lock (Lock)
            {
                base.Invoke(action);
            }
        }

        public override void Invoke<T>(Action<T> action, T param)
        {
            lock (Lock)
            {
                base.Invoke(action, param);
            }
        }
    }
}