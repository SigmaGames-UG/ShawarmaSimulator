using ED262C;
using UnityEngine;

public class SimpleLinkedQueue<T> : ISimpleQueue<T>
{
    LinkedNode<T> first;
    LinkedNode<T> last;

    public int Count { get; private set; }
    
    public bool IsEmpty 
    {
        get { return first == null; }
    }

    public T Dequeue()
    {
        T item = first.value;
        first = first.next;

        if (first == null)
        {
            last = null;
        }
        else
        {
            first.prev = null;
        }
        Count--;
        return item;
    }

    public void Enqueue(T Item)
    {
        LinkedNode<T> newNode = new LinkedNode<T>(Item);

        if (IsEmpty)
        {
            first = newNode;
            last = newNode;
        }
        else
        {
            last.next = newNode;
            newNode.prev = last;
            last = newNode;
        }
        Count++;
    }

    public T Peek()
    {
        return first.value;
    }
}
