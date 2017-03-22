namespace CMA.Messages.Mediators
{
    public interface IMessageMediator
    {
        string Key { get; }
        void TransmitMessage(IMessage message);
    }
}