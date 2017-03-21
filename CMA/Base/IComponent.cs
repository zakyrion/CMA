using System;
using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;

namespace CMA
{
    public interface IComponent<T> : ICloneable, IMessageRespounder
    {
        List<IMessageHandler> ToOwnerMessages { get; }
        List<IRequestHandler> ToOwnerRequests { get; }
        List<IMessageMarkerHandler> ToOwnerMessageMarkers { get; }
        List<IRequestMarkerHandler> ToOwnerRequestMarkers { get; }
        T Key { get; }
        ICompositor<T> Owner { get; }
        void OnAdd(ICompositor<T> owner);
        void OnRemove();
    }
}