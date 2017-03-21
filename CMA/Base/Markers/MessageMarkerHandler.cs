using System;
using CMA.Messages;

namespace CMA.Markers
{
    public class MessageMarkerHandler<T> : IMessageMarkerHandler where T : IMarker
    {
        protected MessageMarkerDelegate<IMessage, T> DelegateField;

        public MessageMarkerHandler(MessageMarkerDelegate<IMessage, T> @delegate)
        {
            DelegateField = @delegate;
        }

        public bool Equals(IMessageMarkerHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public void Invoke(IMessage message)
        {
            DelegateField.Invoke(message, message.GetMarker<T>());
        }

        public string Key
        {
            get { return typeof(T).ToString(); }
        }

        public Delegate Delegate
        {
            get { return DelegateField; }
        }

        public bool Contains(Delegate @delegate)
        {
            return @delegate == (Delegate)DelegateField;
        }
    }
}