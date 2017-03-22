namespace CMA.Markers
{
    public interface IRequestMarkerHandler : IMarkerHandler
    {
        bool Equals(IRequestMarkerHandler handler);
        void Invoke(IRequest request);
    }
}