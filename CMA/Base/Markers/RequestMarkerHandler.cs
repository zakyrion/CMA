using System;
using CMA.Messages;

namespace CMA.Markers
{
    public class RequestMarkerHandler<T> : IRequestMarkerHandler
    {
        protected RequestDelegate<object, IRequest> DelegateField;

        public RequestMarkerHandler(RequestDelegate<object, IRequest> delegateField)
        {
            DelegateField = delegateField;
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

        public bool Equals(IRequestMarkerHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public object Invoke(IRequest request)
        {
            return DelegateField(request);
        }
    }
}