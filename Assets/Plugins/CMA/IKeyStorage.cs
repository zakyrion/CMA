public interface IKeyStorage
{
    void AddKVP(object key, object value);
    void Remove(object key);
    bool Contain(object key);
    T Get<T>(object key);
}