namespace CMA.Markers
{
    public abstract class Marker<T> : IMarker
    {
        protected Marker(T key)
        {
            Key = key;
        }

        public virtual T Key { get; protected set; }

        public object ObjKey { get { return Key; } }

        public virtual string MarkerKey
        {
            get { return GetType().Name; }
        }
    }
}