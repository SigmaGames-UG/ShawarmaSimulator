using System;

public class SimpleArrayPriorityQueue<T> : ISimplePriorityQueue<T>
{
    private T[] items;
    // Items guarda los objetos de la tienda
    private int[] priorities;
    // priorities guarda las prioridades
    private int count;

    private const int defaultCapacity = 10;
    
    public int Count => count;
    public bool IsEmpty => count == 0;

    public SimpleArrayPriorityQueue(int capacity = defaultCapacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        items = new T[capacity];
        priorities = new int[capacity];
        count = 0;
    }

    // Enqueue  mete los objetos de la tienda ordenados
    public void Enqueue(T item, int priority)
    {
        ValidateSize();

        int insertIndex = count;

        for (int i = count;
             i > 0 && priority < priorities[i - 1];
             i--)
        {
            items[i] = items[i - 1];
            priorities[i] = priorities[i - 1];

            insertIndex = i - 1;
        }

        items[insertIndex] = item;
        priorities[insertIndex] = priority;

        count++;
    }
    // Dequeue saca el primero y mueve el resto a la izquierda
    public T Dequeue()
    {
        ValidateNotEmpty();

        T firstItem = items[0];

        ShiftLeft();
        count--;

     
        items[count] = default(T);
        priorities[count] = 0;

        return firstItem;
    }

    // Peek revisa/consulta el primero
    public T Peek()
    {
        ValidateNotEmpty();
        return items[0];
    }

    public int GetHighestPriority()
    {
        ValidateNotEmpty();
        return priorities[0];
    }

    public void Clear()
    {
        for (int i = 0; i < count; i++)
        {
            items[i] = default(T);
            priorities[i] = 0;
        }

        count = 0;
    }

    public T[] ToArray()
    {
        T[] copy = new T[count];

        for (int i = 0; i < count; i++)
        {
            copy[i] = items[i];
        }

        return copy;
    }

    private void ValidateSize()
    {
        if (count < items.Length)
        {
            return;
        }

        int newCapacity = items.Length == 0
            ? defaultCapacity
            : items.Length * 2;

        Resize(newCapacity);
    }

    private void Resize(int newCapacity)
    {
        T[] newItems = new T[newCapacity];
        int[] newPriorities = new int[newCapacity];

        for (int i = 0; i < count; i++)
        {
            newItems[i] = items[i];
            newPriorities[i] = priorities[i];
        }

        items = newItems;
        priorities = newPriorities;
    }

    private void ShiftLeft()
    {
        for (int i = 1; i < count; i++)
        {
            items[i - 1] = items[i];
            priorities[i - 1] = priorities[i];
        }
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