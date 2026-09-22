namespace ED262C
{
    public class LinkedNode<T>
    {
        // Referencias a los nodos vecinos
        public LinkedNode<T> prev;
        public LinkedNode<T> next;

        // El dato que realmente guarda el nodo
        public T value;

        // Constructor que recibe por parametro el valor inicial
        public LinkedNode(T value)
        {
            this.value = value;
        }
    }
}
