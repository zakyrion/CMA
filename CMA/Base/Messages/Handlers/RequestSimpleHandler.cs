using System;

namespace CMA.Messages
{
    public class RequestSimpleHandler<T> : IRequestHandler
    {
        protected RequestSimpleDelegate<T> DelegateField;

        public RequestSimpleHandler(RequestSimpleDelegate<T> delegateField)
        {
            DelegateField = delegateField;
        }

        public string Key
        {
            get { return typeof (T).Name; }
        }

        public string MessageKey
        {
            get { return null; }
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

        public void Invoke(IRequest request)
        {
            var result = DelegateField();
            request.Done(result);
        }
    }
}