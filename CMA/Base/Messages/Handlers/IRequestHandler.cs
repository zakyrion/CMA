using System;

namespace CMA.Messages
{
    public interface IRequestHandler
    {
        string Key { get; }
        string MessageKey { get; }
        Delegate Delegate { get; }
        bool Contains(Delegate @delegate);
        bool Equals(IRequestHandler handler);
        void Invoke(IRequest request);
    }
}