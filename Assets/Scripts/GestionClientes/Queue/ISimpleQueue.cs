using UnityEngine;

public interface ISimpleQueue<T>
{
    public void Enqueue(T Item);
    public T Peek();
    public T Dequeue();
    public bool IsEmpty { get; }
}
