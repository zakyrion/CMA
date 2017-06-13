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
using System.Threading;

namespace CMA.Messages
{
    public sealed class SimpleRequest<T> : Communication, IRequest
    {
        public SimpleRequest()
        {
            ResultKey = typeof (T).Name;
            Result = default(T);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public T CastResult
        {
            get { return (T) Result; }
        }

        public object Result { get; private set; }
        public string ResultKey { get; private set; }

        public RequestKey? RequestKey
        {
            get { return null; }
        }

        public ManualResetEvent Sync { get; set; }

        public void Done(object result)
        {
            Result = result;

            if (Sync != null)
                Sync.Set();
        }

        public int ThreadId { get; private set; }

        public override void Fail()
        {
            base.Fail();

            if (Sync != null)
                Sync.Set();

            throw new Exception("Request Failed");
        }
    }
}