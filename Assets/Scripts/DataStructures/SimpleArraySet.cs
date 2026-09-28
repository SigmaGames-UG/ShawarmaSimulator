using System;

public class SimpleArraySet<T> : ISimpleSet<T>
{
    private T[] items;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public SimpleArraySet(int capacity = 10)
    {
        items = new T[capacity];
        count = 0;
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < count; i++)
        {
            if (items[i].Equals(item)) return true;
        }
        return false;
    }

    public bool Add(T item)
    {
        if (Contains(item)) return false; 

        if (count >= items.Length) Resize(items.Length * 2);

        items[count] = item;
        count++;
        return true;
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < count; i++)
        {
            if (items[i].Equals(item))
            {
                items[i] = items[count - 1];
                items[count - 1] = default(T);
                count--;
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        Array.Clear(items, 0, count);
        count = 0;
    }

    public T[] ToArray()
    {
        T[] result = new T[count];
        Array.Copy(items, result, count);
        return result;
    }

    private void Resize(int newSize)
    {
        T[] newArray = new T[newSize];
        Array.Copy(items, newArray, count);
        items = newArray;
    }
}