using System;
using System.Collections.Generic;
using CMA.Markers;
using CMA.Messages;
using CMA.Messages.Mediators;

namespace CMA
{
    public interface IComponent<T> : ICloneable, IMessageRespounder
    {
        List<IMessageMediator> ToGlobalMessages { get; }
        List<IRequestMediator> ToGlobalRequests { get; }
        List<IMessageMediator> ToOwnerMessages { get; }
        List<IRequestMediator> ToOwnerRequests { get; }
        List<IMessageMarkerHandler> ToOwnerMessageMarkers { get; }
        List<IRequestMarkerHandler> ToOwnerRequestMarkers { get; }
        T Key { get; }
        ICompositor<T> Owner { get; }
        void OnAdd(ICompositor<T> owner);
        void OnRemove();
    }
}