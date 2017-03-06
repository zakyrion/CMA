public abstract class Request : Communication, IRequest
{
    public virtual IRequest Initalize<R>()
    {
        ResultKey = typeof (R).Name;
        RequestKey = new RequestKey(ResultKey, GetType().Name);
        return this;
    }

    public string ResultKey { get; protected set; }
    public RequestKey? RequestKey { get; protected set; }
}