public interface ISimpleSet<T>
{
    bool Add(T item);
    bool Remove(T item);
    bool Contains(T item);
    void Clear();
    int Count { get; }
    bool IsEmpty { get; }
    T[] ToArray(); 
}