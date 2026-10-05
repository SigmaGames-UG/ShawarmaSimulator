public interface ISimpleDictionary<TKey, TValue>
{
    TValue this[TKey key] { get; set; }

    int Count { get; }

    bool IsEmpty { get; }

    void Add(TKey key, TValue value);

    bool TryAdd(TKey key, TValue value);

    bool Remove(TKey key);

    bool ContainsKey(TKey key);

    bool TryGetValue(TKey key, out TValue value);

    void Clear();

    TKey[] Keys();

    TValue[] Values();
}