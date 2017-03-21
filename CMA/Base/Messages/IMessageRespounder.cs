namespace CMA.Messages
{
    public interface IMessageRespounder
    {
        void SendMessage(IMessage message);
        bool ContainsMessage<T>() where T : IMessage;
        bool ContainsMessage(IMessage message);

        object SendRequest(IRequest request);
        T SendRequest<T>(IRequest request);
        T SendRequest<T>();
        bool ContainsRequest<T>();
        bool ContainsRequest(IRequest request);
    }
}