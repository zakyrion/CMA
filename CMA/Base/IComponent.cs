using System;
using CMA.Messages;

namespace CMA
{
    public interface IComponent<T> : ICloneable, IMessageRespounder
    {
        T Key { get; }
        ICompositor<T> Owner { get; }
        void OnAdd(ICompositor<T> owner);
        void OnRemove();
        void Quit();
        void SubscribeGlobal();
        void RemoveGlobal();
    }
}