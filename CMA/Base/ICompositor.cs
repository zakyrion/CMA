using System.Collections.Generic;
using CMA.Messages;

namespace CMA
{
    public interface ICompositor<K> : IMessageManager
    {
        IMessageManager System { get; }

        bool Contains(K key);
        bool Contains<T>(T component) where T : IComponent<K>;
        void AddComponent<T>(T component) where T : IComponent<K>;
        void RemoveComponent<T>(T component) where T : IComponent<K>;
        T GetComponent<T>(K key);
        T GetComponent<T>();
        List<T> GetComponents<T>(K key);
    }
}