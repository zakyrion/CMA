namespace CMA.Messages.Mediators
{
    public class MessageMediator<T> : IMessageMediator
    {
        protected IMessageManager Owner;

        public MessageMediator(IMessageManager owner)
        {
            Key = typeof (T).Name;
            Owner = owner;
        }

        public virtual string Key { get; protected set; }

        public void TransmitMessage(IMessage message)
        {
            Owner.SendMessage(message);
        }
    }
}