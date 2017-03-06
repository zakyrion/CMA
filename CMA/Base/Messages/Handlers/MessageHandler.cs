using System;

namespace CMA.Messages
{
    public class MessageHandler<T> : IMessageHandler where T : IMessage
    {
        protected MessageDelegate<T> DelegateField;

        public MessageHandler(MessageDelegate<T> @delegate)
        {
            DelegateField = @delegate;
        }

        public string Key
        {
            get { return typeof (T).Name; }
        }

        public Delegate Delegate
        {
            get { return DelegateField; }
        }

        public bool Contains(Delegate @delegate)
        {
            return @delegate == (Delegate) DelegateField;
        }

        public bool Equals(IMessageHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public void Invoke(IMessage message)
        {
            DelegateField((T) message);
        }
    }
}