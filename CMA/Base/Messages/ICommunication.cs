using System.Collections.Generic;
using CMA.Markers;

public interface ICommunication
{
    bool IsFaild { get; }
    List<IMarker> Markers { get; }
    List<IMarker> ReturningMarkers { get; }
    void AddMarkerForReturn(IMarker marker);
    void AddMarker(IMarker marker);
    T GetMarker<T>() where T : IMarker;
    T GetReturningMarker<T>() where T : IMarker;
    bool Contains<T>();
    void AddMarkers(IEnumerable<IMarker> markers);
    void Fail();
}