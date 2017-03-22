namespace CMA.Messages.Mediators
{
    public class RequestMediatorM<T, K> : SimpleRequestMediator<T>
    {
        public RequestMediatorM(IMessageManager owner) : base(owner)
        {
            RequestKey = new RequestKey(ResultKey, typeof (K).Name);
        }
    }
}