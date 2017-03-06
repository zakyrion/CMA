namespace CMA.Markers
{
    public interface IMessageMarkerHandler : IMarkerHandler
    {
        bool Equals(IMessageMarkerHandler handler);
        void Invoke(IMessage message);
    }
}