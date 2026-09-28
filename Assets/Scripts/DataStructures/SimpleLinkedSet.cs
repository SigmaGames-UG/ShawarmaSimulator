public class SimpleLinkedSet<T> : ISimpleSet<T>
{
    private class Node
    {
        public T Value;
        public Node Next;
        public Node(T value) { Value = value; }
    }

    private Node head;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public bool Contains(T item)
    {
        Node current = head;
        while (current != null)
        {
            if (current.Value.Equals(item)) return true;
            current = current.Next;
        }
        return false;
    }

    public bool Add(T item)
    {
        if (Contains(item)) return false;

        Node newNode = new Node(item);
        newNode.Next = head;
        head = newNode;
        count++;
        return true;
    }

    public bool Remove(T item)
    {
        Node current = head;
        Node previous = null;

        while (current != null)
        {
            if (current.Value.Equals(item))
            {
                if (previous == null) head = current.Next;
                else previous.Next = current.Next;

                count--;
                return true;
            }
            previous = current;
            current = current.Next;
        }
        return false;
    }

    public void Clear()
    {
        head = null;
        count = 0;
    }

    public T[] ToArray()
    {
        T[] result = new T[count];
        Node current = head;
        int i = 0;
        while (current != null)
        {
            result[i++] = current.Value;
            current = current.Next;
        }
        return result;
    }
}