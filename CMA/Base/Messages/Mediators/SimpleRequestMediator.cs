namespace CMA.Messages.Mediators
{
    public class SimpleRequestMediator<T> : IRequestMediator
    {
        protected IMessageManager Owner;

        public SimpleRequestMediator(IMessageManager owner)
        {
            Owner = owner;
            ResultKey = typeof (T).Name;
        }

        public string ResultKey { get; protected set; }
        public RequestKey? RequestKey { get; protected set; }

        public void TransmitRequest(IRequest request)
        {
            Owner.SendRequest(request);
        }
    }
}