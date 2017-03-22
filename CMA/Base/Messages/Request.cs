using System.Threading;

public abstract class Request<R> : Communication, IRequest
{
    public R CastResult
    {
        get { return (R) Result; }
    }

    public virtual IRequest Initalize()
    {
        ResultKey = typeof (R).Name;
        RequestKey = new RequestKey(ResultKey, GetType().Name);
        return this;
    }

    public void Done(object result)
    {
        Result = result;

        if (Mutex != null)
            Mutex.ReleaseMutex();
    }

    public override void Fail()
    {
        base.Fail();

        if (Mutex != null)
            Mutex.ReleaseMutex();
    }

    public object Result { get; protected set; }
    public string ResultKey { get; protected set; }
    public RequestKey? RequestKey { get; protected set; }
    public Mutex Mutex { get; set; }
}