using System.Collections.Generic;
using System.Threading;

namespace CMA.Messages
{
    public class ThreadMessageManager : MessageManager
    {
        protected object _lock = new object();
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
            lock (_lock)
            {
                Messages.Enqueue(message);
                Monitor.Pulse(_lock);
            }
        }

        protected override void TransmitRequest(IRequest request)
        {
            if (request.ThreadId == Thread.ManagedThreadId)
                base.TransmitRequest(request);
            else
            {
                bool isNeedToWait = false;

                lock (_lock)
                {
                    if (request.Sync == null)
                    {
                        isNeedToWait = true;
                        request.Sync = new ManualResetEvent(false);
                    }

                    Requests.Enqueue(request);
                    Monitor.Pulse(_lock);
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

                lock (_lock)
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
                            Monitor.Pulse(_lock);
                            Monitor.Wait(_lock);
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