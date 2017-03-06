namespace CMA.Messages
{
    public sealed class SimpleRequest<T> : Communication, IRequest
    {
        public SimpleRequest()
        {
            ResultKey = typeof (T).Name;
        }

        public string ResultKey { get; private set; }

        public RequestKey? RequestKey
        {
            get { return null; }
        }

        public IRequest Initalize<R>()
        {
            return this;
        }
    }
}