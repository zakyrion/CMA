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
            lock (Messages)
            {
                Messages.Enqueue(message);
                Monitor.Pulse(Messages);
            }
        }

        public override T SendRequest<T>(IRequest request)
        {
            lock (Requests)
            {
                Requests.Enqueue(request);
            }

            Thread.Sleep(Timeout.Infinite);
            return base.SendRequest<T>(request);
        }

        protected void Update()
        {
            while (true)
            {
                IMessage[] messages = null;

                lock (Messages)
                {
                    do
                    {
                        if (Messages.Count > 0)
                        {
                            messages = Messages.ToArray();
                            Messages.Clear();
                        }
                        else
                        {
                            Monitor.Pulse(Messages);
                            Monitor.Wait(Messages);
                        }

                    } while (messages != null);
                }

                foreach (var message in messages)
                    base.SendMessage(message);
            }
        }
    }
}