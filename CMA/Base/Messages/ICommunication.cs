using System.Collections.Generic;
using CMA.Markers;

public interface ICommunication
{
    List<Marker> Markers { get; }
    List<Marker> ReturningMarkers { get; }
    void AddMarkerForReturn(Marker marker);
    void AddMarker(Marker marker);
    T GetMarker<T>() where T : Marker;
    T GetReturningMarker<T>() where T : Marker;
    bool Contains<T>();
    void AddMarkers(IEnumerable<Marker> markers);
}