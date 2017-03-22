using System;
using CMA.Messages;

namespace CMA.Markers
{
    public class RequestMarkerHandler<T> : IRequestMarkerHandler where T : IMarker
    {
        protected RequestMarkerDelegate<object, IRequest, T> DelegateField;

        public RequestMarkerHandler(RequestMarkerDelegate<object, IRequest, T> delegateField)
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

        public void Invoke(IRequest request)
        {
            DelegateField(request, request.GetMarker<T>());
        }
    }
}