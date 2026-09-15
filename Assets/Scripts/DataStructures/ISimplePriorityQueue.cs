public interface ISimplePriorityQueue<T>
{
    int Count { get; }
    bool IsEmpty { get; }

    void Enqueue(T item, int priority);
    T Dequeue();
    T Peek();
    int GetHighestPriority();

    void Clear();
    T[] ToArray();
}