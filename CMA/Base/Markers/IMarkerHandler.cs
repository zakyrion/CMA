using System;

namespace CMA.Markers
{
    public interface IMarkerHandler
    {
        string Key { get; }
        Delegate Delegate { get; }
        bool Contains(Delegate @delegate);
    }
}