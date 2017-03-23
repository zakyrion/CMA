using CMA.Messages;

namespace CMA
{
    public abstract class ActorSystem<T> : Compositor<T>
    {
        protected ActorSystem()
        {
            System = MessageManager;
        }

        protected ActorSystem(IMessageManager messageManager) : base(messageManager)
        {
            System = MessageManager;
        }

        public override void SendMessage(IMessage message)
        {
            MessageManager.SendMessage(message);
        }
    }
}