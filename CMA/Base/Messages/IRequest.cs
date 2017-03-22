using System.Threading;

public interface IRequest : ICommunication
{
    object Result { get; }
    string ResultKey { get; }
    RequestKey? RequestKey { get; }
    Mutex Mutex { get; set; }
    IRequest Initalize();
    void Done(object result);
}