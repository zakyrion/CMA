using System.Threading;

public abstract class Request<R> : Communication, IRequest
{
    public R CastResult
    {
        get { return (R)Result; }
    }

    protected Request()
    {
        ResultKey = typeof(R).Name;
        RequestKey = new RequestKey(ResultKey, GetType().Name);
        Result = default(R);
        ThreadId = Thread.CurrentThread.ManagedThreadId;
    }

    public void Done(object result)
    {
        Result = result;

        if (Sync != null)
            Sync.Set();
    }

    public int ThreadId { get; protected set; }

    public override void Fail()
    {
        base.Fail();

        if (Sync != null)
            Sync.Set();
    }

    public object Result { get; protected set; }
    public string ResultKey { get; protected set; }
    public RequestKey? RequestKey { get; protected set; }
    public ManualResetEvent Sync { get; set; }
}