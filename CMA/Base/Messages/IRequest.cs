using System.Threading;

public interface IRequest : ICommunication
{
    object Result { get; }
    string ResultKey { get; }
    RequestKey? RequestKey { get; }
    ManualResetEvent Sync { get; set; }
    void Done(object result);
    int ThreadId { get; }
}