public class SimpleLinkedList<T> : ISimpleList<T>
{
    private class Node
    {
        public T Data;
        public Node Next;
        public Node(T data) { Data = data; }
    }

    private Node head;
    private int count;
    public int Count => count;

    public void Add(T item)
    {
        Node newNode = new Node(item);
        if (head == null) head = newNode;
        else
        {
            Node current = head;
            while (current.Next != null) current = current.Next;
            current.Next = newNode;
        }
        count++;
    }

    public T Get(int index)
    {
        Node current = head;
        for (int i = 0; i < index; i++) current = current.Next;
        return current.Data;
    }

    public void RemoveAt(int index)
    {
        if (index == 0) head = head.Next;
        else
        {
            Node current = head;
            for (int i = 0; i < index - 1; i++) current = current.Next;
            current.Next = current.Next.Next;
        }
        count--;
    }
}