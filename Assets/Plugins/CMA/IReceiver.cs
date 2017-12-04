using CMA.Messages;

public interface IReceiver
{
    void PushMessage(IMessage message);
}