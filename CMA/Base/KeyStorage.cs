using System.Collections.Generic;
using UnityEngine;

public class KeyStorage<T> : IKeyStorage
{
    protected Dictionary<T, object> Cache = new Dictionary<T, object>();

    public void AddKVP(object key, object value)
    {
        Cache.Add((T) key, value);
    }

    public void Remove(object key)
    {
        if (Contain(key))
            Cache.Remove((T) key);
    }

    public bool Contain(object key)
    {
        return Cache.ContainsKey((T) key);
    }

    public T1 Get<T1>(object key)
    {
        return (T1) Cache[(T) key];
    }
}