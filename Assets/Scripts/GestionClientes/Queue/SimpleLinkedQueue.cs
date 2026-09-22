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
        //agarra el primer nodo que entró a la queue
        T item = first.value;
        //convierte el nuevo primer nodo al siguiente del que va a salir
        first = first.next;

        //Si no hay más elementos en la queue, acá se declara
        if (first == null)
        {
            last = null;
        }
        //Si quedan elementos, se debe nullear el prev del que está ahora primero
        else
        {
            first.prev = null;
        }
        //Devuelve el item
        Count--;
        return item;
    }

    public void Enqueue(T Item)
    {
        //Crea un nuevo nodo
        LinkedNode<T> newNode = new LinkedNode<T>(Item);

        //Si la lista está vacía, el nodo es first y last
        if (IsEmpty)
        {
            first = newNode;
            last = newNode;
        }
        //Si no, el nodo se vuelve el siguiente del último, el último se vuelve el previo del nuevo, y el nuevo se vuelve el último.
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
