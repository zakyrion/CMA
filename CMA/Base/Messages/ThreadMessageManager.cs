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
using System.Collections.Generic;
using System.Threading;

namespace CMA.Messages
{
    public class ThreadMessageManager : MessageManager
    {
        protected Queue<IMessage> Messages = new Queue<IMessage>();
        protected Queue<IRequest> Requests = new Queue<IRequest>();
        protected Thread Thread;

        public ThreadMessageManager()
        {
            Thread = new Thread(Update);
            Thread.Start();
        }

        public override IMessageManager NewWithType()
        {
            return new ThreadMessageManager();
        }

        public override void SendMessage(IMessage message)
        {
            lock (Lock)
            {
                Messages.Enqueue(message);
                Monitor.Pulse(Lock);
            }
        }

        protected override void TransmitRequest(IRequest request)
        {
            if (request.ThreadId == Thread.ManagedThreadId)
                base.TransmitRequest(request);
            else
            {
                bool isNeedToWait = false;

                lock (Lock)
                {
                    if (request.Sync == null)
                    {
                        isNeedToWait = true;
                        request.Sync = new ManualResetEvent(false);
                    }

                    Requests.Enqueue(request);
                    Monitor.Pulse(Lock);
                }

                if (isNeedToWait)
                    request.Sync.WaitOne();
            }
        }

        protected void Update()
        {
            while (true)
            {
                IMessage[] messages = null;
                IRequest[] requests = null;

                lock (Lock)
                {
                    do
                    {
                        if (Messages.Count > 0)
                        {
                            messages = Messages.ToArray();
                            Messages.Clear();
                        }
                        else if (Requests.Count > 0)
                        {
                            requests = Requests.ToArray();
                            Requests.Clear();
                        }
                        else
                        {
                            Monitor.Pulse(Lock);
                            Monitor.Wait(Lock);
                        }

                    } while (messages == null && requests == null);
                }

                if (requests != null)
                {
                    foreach (var request in requests)
                        base.TransmitRequest(request);
                }

                if (messages != null)
                {
                    foreach (var message in messages)
                        base.SendMessage(message);
                }
            }
        }

        public override void Quit()
        {
            Thread.Abort();
            base.Quit();
        }
    }
}