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
using UnityEngine;

namespace CMA.Messages
{
    public class Message : Communication, IMessage
    {
        protected readonly IActionHandler Action;
        protected object Lock = new object();

        public Message(object data, IActionHandler action = null)
        {
            Action = action;
            Data = data;
        }

        public object Data { get; protected set; }

        public virtual bool IsDone { get; protected set; }
        public IMessageManager MessageManager { get; set; }

        public virtual void Done(object param = null)
        {
            InvokeChainAction(param);
        }

        public virtual string GetKey()
        {
            return Data.GetType().ToString();
        }

        public virtual void LockMessage()
        {
        }

        public virtual void UnlockMessage()
        {
        }

        public T GetData<T>()
        {
            return (T) Data;
        }

        public virtual void InvokeChainAction(object param)
        {
            if (Action != null)
                Action.Invoke(MessageManager, param);
        }

        public void ShowTrace()
        {
            Debug.Log("Message: " + GetKey());
            foreach (var trace in Traces)
                Debug.Log("Trace: " + trace);
        }
    }

    public class SingletonMessage : Message
    {
        private int? _threadId;
        protected AutoResetEvent Event = new AutoResetEvent(true);

        public SingletonMessage(object data, IActionHandler action = null) : base(data, action)
        {
        }

        public override bool IsDone
        {
            get
            {
                lock (Lock)
                {
                    return base.IsDone;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    base.IsDone = value;
                }
            }
        }

        public override void Done(object param = null)
        {
            IsDone = true;
            base.Done(param);
        }

        public override void LockMessage()
        {
            if (_threadId.HasValue)
            {
                if (_threadId.Value == Thread.CurrentThread.ManagedThreadId)
                    return;
            }
            else
            {
                _threadId = Thread.CurrentThread.ManagedThreadId;
            }

            Event.WaitOne();
        }

        public override void UnlockMessage()
        {
            Event.Set();
        }
    }
}