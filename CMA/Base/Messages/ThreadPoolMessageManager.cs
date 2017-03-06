using System.Threading;

namespace CMA.Messages
{
    public class ThreadPoolMessageManager: MessageManager
    {
        public override IMessageManager NewWithType()
        {
            return new ThreadPoolMessageManager();
        }

        public override void SendMessage(IMessage message)
        {
            ThreadPool.QueueUserWorkItem(state => { base.SendMessage(message); });
        }
    }
}