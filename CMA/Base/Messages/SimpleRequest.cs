using System.Threading;

namespace CMA.Messages
{
    public sealed class SimpleRequest<T> : Communication, IRequest
    {
        public SimpleRequest()
        {
            ResultKey = typeof (T).Name;
        }

        public T CastResult
        {
            get { return (T) Result; }
        }

        public object Result { get; private set; }
        public string ResultKey { get; }

        public RequestKey? RequestKey
        {
            get { return null; }
        }

        public Mutex Mutex { get; set; }

        public IRequest Initalize()
        {
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
    }
}