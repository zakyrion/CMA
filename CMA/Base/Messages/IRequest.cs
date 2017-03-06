public interface IRequest : ICommunication
{
    string ResultKey { get; }
    RequestKey? RequestKey { get; }
    IRequest Initalize<R>();
}