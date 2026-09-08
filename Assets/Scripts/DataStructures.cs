using UnityEngine;

// 1. ISimpleStack.cs
public interface ISimpleStack<T>
{
    void Push(T item);
    T Pop();
    T Peek();
    bool IsEmpty();
    int Count { get; }
}

// 2. SimpleArrayStack.cs
public class SimpleArrayStack<T> : ISimpleStack<T>
{
    private T[] array;
    private int top;

    public SimpleArrayStack(int capacity)
    {
        array = new T[capacity];
        top = -1;
    }

    public int Count => top + 1;
    public bool IsEmpty() => top == -1;

    public void Push(T item)
    {
        if (top < array.Length - 1) array[++top] = item;
    }

    public T Pop()
    {
        if (IsEmpty()) return default;
        return array[top--];
    }

    public T Peek()
    {
        if (IsEmpty()) return default;
        return array[top];
    }
}

// 3. SimpleLinkedStack.cs
public class SimpleLinkedStack<T> : ISimpleStack<T>
{
    private class Node
    {
        public T Data;
        public Node Next;
    }

    private Node top;
    private int count;

    public int Count => count;
    public bool IsEmpty() => top == null;

    public void Push(T item)
    {
        top = new Node { Data = item, Next = top };
        count++;
    }

    public T Pop()
    {
        if (IsEmpty()) return default;
        T data = top.Data;
        top = top.Next;
        count--;
        return data;
    }

    public T Peek() => IsEmpty() ? default : top.Data;
}