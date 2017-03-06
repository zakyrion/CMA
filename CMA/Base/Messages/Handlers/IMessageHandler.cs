using System;

namespace CMA.Messages
{
    public interface IMessageHandler
    {
        string Key { get; }
        Delegate Delegate { get; }
        bool Contains(Delegate @delegate);
        bool Equals(IMessageHandler handler);
        void Invoke(IMessage message);
    }
}