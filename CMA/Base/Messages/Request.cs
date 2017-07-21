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

namespace CMA.Messages
{
    /// <summary>
    /// </summary>
    /// <typeparam name="R">Result type</typeparam>
    public abstract class Request : SingletonMessage
    {
        protected Request(IActionHandler action) : base(action)
        {
        }

        public override void Done(object result = null)
        {
            if (result != null && !IsDone)
            {
                base.Done();
                InvokeChainAction(result);
            }
        }
    }

    public abstract class Request<R> : Request
    {
        protected Request(IActionHandler action, R data) : base(action)
        {
            Data = data;
        }

        public R Data { get; protected set; }
    }

    public class SimpleRequest<R> : Request
    {
        public SimpleRequest(Action<R> action) : base(new ActionHandler<R>(action))
        {
        }

        public override string GetKey()
        {
            return typeof(R).ToString();
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="R">Result type</typeparam>
    /// <typeparam name="T">Data param</typeparam>
    public abstract class Request<R, T> : Request
    {
        protected Request(IActionHandler action, T data) : base(action)
        {
            Data = data;
        }

        public T Data { get; protected set; }

        public override string GetKey()
        {
            return typeof(R).ToString();
        }
    }
}