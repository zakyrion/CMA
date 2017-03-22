namespace CMA.Messages.Mediators
{
    public interface IRequestMediator
    {
        string ResultKey { get; }
        RequestKey? RequestKey { get; }
        void TransmitRequest(IRequest request);
    }
}