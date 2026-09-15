using System;

public class SimpleLinkedPriorityQueue<T> : ISimplePriorityQueue<T>
{
    private class PriorityLinkedNode
    {
        public T item;
        public int priority;

        public PriorityLinkedNode next;
        public PriorityLinkedNode prev;

        public PriorityLinkedNode(T item, int priority)
        {
            this.item = item;
            this.priority = priority;
        }
    }

    private PriorityLinkedNode first;
    private PriorityLinkedNode last;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public void Enqueue(T item, int priority)
    {
        PriorityLinkedNode newNode =
            new PriorityLinkedNode(item, priority);

        if (IsEmpty)
        {
            
            first = newNode;
            last = newNode;
        }
        else if (priority < first.priority)
        {
           
            newNode.next = first;
            first.prev = newNode;
            first = newNode;
        }
        else
        {
           
            PriorityLinkedNode current = last;

            while (current.prev != null &&
                   priority < current.priority)
            {
                current = current.prev;
            }

            
            newNode.prev = current;
            newNode.next = current.next;

            if (current.next != null)
            {
                current.next.prev = newNode;
            }
            else
            {
                last = newNode;
            }

            current.next = newNode;
        }

        count++;
    }

    public T Dequeue()
    {
        ValidateNotEmpty();

        PriorityLinkedNode removedNode = first;
        T firstItem = removedNode.item;

        first = removedNode.next;

        if (first != null)
        {
            first.prev = null;
        }
        else
        {
           
            last = null;
        }

        removedNode.next = null;
        count--;

        return firstItem;
    }

    public T Peek()
    {
        ValidateNotEmpty();
        return first.item;
    }

    public int GetHighestPriority()
    {
        ValidateNotEmpty();
        return first.priority;
    }

    public void Clear()
    {
        first = null;
        last = null;
        count = 0;
    }

    public T[] ToArray()
    {
        T[] copy = new T[count];
        PriorityLinkedNode current = first;

        for (int i = 0; i < count; i++)
        {
            copy[i] = current.item;
            current = current.next;
        }

        return copy;
    }

    private void ValidateNotEmpty()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "La cola de prioridad está vacía.");
        }
    }
}