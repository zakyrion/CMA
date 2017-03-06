namespace CMA
{
    public abstract class Builder<T> : IBuilder
    {
        public string Key { get { return typeof(T).Name; } }
        public abstract object Build();
        public abstract object Build(IMessage message);
    }
}