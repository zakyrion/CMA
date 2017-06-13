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
using System.Threading;

namespace CMA.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="R">Result type</typeparam>
    public abstract class Request<R> : Communication, IRequest
    {
        protected Request()
        {
            ResultKey = typeof(R).Name;
            RequestKey = new RequestKey(ResultKey, GetType().Name);
            Result = default(R);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public R CastResult
        {
            get { return (R)Result; }
        }

        public void Done(object result)
        {
            Result = result;

            if (Sync != null)
                Sync.Set();
        }

        public int ThreadId { get; protected set; }

        public override void Fail()
        {
            base.Fail();

            if (Sync != null)
                Sync.Set();
        }

        public object Result { get; protected set; }
        public string ResultKey { get; protected set; }
        public RequestKey? RequestKey { get; protected set; }
        public ManualResetEvent Sync { get; set; }
    }
}