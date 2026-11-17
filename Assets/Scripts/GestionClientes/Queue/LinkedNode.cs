namespace ED262C
{
    public class LinkedNode<T>
    {
        public LinkedNode<T> prev;
        public LinkedNode<T> next;

        public T value;

        public LinkedNode(T value)
        {
            this.value = value;
        }
    }
}
