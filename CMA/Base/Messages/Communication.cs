using System.Collections.Generic;
using CMA.Markers;

public abstract class Communication : ICommunication
{
    protected Dictionary<string, Marker> Cache = new Dictionary<string, Marker>();
    protected Dictionary<string, Marker> CacheReturningMarkers = new Dictionary<string, Marker>();

    protected Communication()
    {
        Markers = new List<Marker>();
        ReturningMarkers = new List<Marker>();
    }

    public List<Marker> Markers { get; protected set; }
    public List<Marker> ReturningMarkers { get; protected set; }

    public T GetMarker<T>() where T : Marker
    {
        var key = typeof (T).Name;
        return (T) Cache[key];
    }

    public T GetReturningMarker<T>() where T : Marker
    {
        var key = typeof (T).Name;
        return (T) CacheReturningMarkers[key];
    }

    public bool Contains<T>()
    {
        var key = typeof (T).Name;
        return Cache.ContainsKey(key);
    }

    public void AddMarkerForReturn(Marker marker)
    {
        ReturningMarkers.Add(marker);
        CacheReturningMarkers[marker.Key] = marker;
    }

    public void AddMarker(Marker marker)
    {
        Markers.Add(marker);
        Cache[marker.Key] = marker;
    }

    public void AddMarkers(IEnumerable<Marker> markers)
    {
        foreach (var marker in markers)
            AddMarker(marker);
    }
}