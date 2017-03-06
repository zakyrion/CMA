using System;

namespace CMA.Messages
{
    public class RequestHandler<T, K> : IRequestHandler where K : IRequest
    {
        protected RequestDelegate<T, K> DelegateField;

        public RequestHandler(RequestDelegate<T, K> delegateField)
        {
            DelegateField = delegateField;
        }

        public string Key
        {
            get { return typeof (T).Name; }
        }

        public string MessageKey
        {
            get { return typeof (K).Name; }
        }

        public Delegate Delegate
        {
            get { return DelegateField; }
        }

        public bool Contains(Delegate @delegate)
        {
            return @delegate == (Delegate) DelegateField;
        }

        public bool Equals(IRequestHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public object Invoke(IRequest request)
        {
            return DelegateField((K) request);
        }
    }
}