namespace CMA.Markers
{
    public interface IRequestMarkerHandler : IMarkerHandler
    {
        bool Equals(IRequestMarkerHandler handler);
        object Invoke(IRequest request);
    }
}