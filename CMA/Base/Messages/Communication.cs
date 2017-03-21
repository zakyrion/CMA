using System.Collections.Generic;
using CMA.Markers;

public abstract class Communication : ICommunication
{
    protected Dictionary<string, IMarker> Cache = new Dictionary<string, IMarker>();
    protected Dictionary<string, IMarker> CacheReturningMarkers = new Dictionary<string, IMarker>();

    protected Communication()
    {
        Markers = new List<IMarker>();
        ReturningMarkers = new List<IMarker>();
    }

    public List<IMarker> Markers { get; protected set; }
    public List<IMarker> ReturningMarkers { get; protected set; }

    public T GetMarker<T>() where T : IMarker
    {
        var key = typeof (T).Name;
        return (T) Cache[key];
    }

    public T GetReturningMarker<T>() where T : IMarker
    {
        var key = typeof (T).Name;
        return (T) CacheReturningMarkers[key];
    }

    public bool Contains<T>()
    {
        var key = typeof (T).Name;
        return Cache.ContainsKey(key);
    }

    public void AddMarkerForReturn(IMarker marker)
    {
        ReturningMarkers.Add(marker);
        CacheReturningMarkers[marker.MarkerKey] = marker;
    }

    public void AddMarker(IMarker marker)
    {
        Markers.Add(marker);
        Cache[marker.MarkerKey] = marker;
    }

    public void AddMarkers(IEnumerable<IMarker> markers)
    {
        foreach (var marker in markers)
            AddMarker(marker);
    }
}