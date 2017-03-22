using System.Threading;

namespace CMA.Messages
{
    public sealed class SimpleRequest<T> : Communication, IRequest
    {
        public SimpleRequest()
        {
            ResultKey = typeof (T).Name;
            Result = default(T);
        }

        public T CastResult
        {
            get { return (T) Result; }
        }

        public object Result { get; private set; }
        public string ResultKey { get; private set; }

        public RequestKey? RequestKey
        {
            get { return null; }
        }

        public ManualResetEvent Sync { get; set; }

        public void Done(object result)
        {
            Result = result;

            if (Sync != null)
                Sync.Set();
        }

        public override void Fail()
        {
            base.Fail();

            if (Sync != null)
                Sync.Set();
        }
    }
}