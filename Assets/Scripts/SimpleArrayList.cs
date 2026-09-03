public class SimpleArrayList<T> : ISimpleList<T>
{
    private T[] array;
    private int count;
    public int Count => count;

    public SimpleArrayList(int capacity = 10)
    {
        array = new T[capacity];
        count = 0;
    }
    
    public void Add(T item)
    {
        if (count >= array.Length)
        {
            T[] newArray = new T[array.Length * 2];
            for (int i = 0; i < array.Length; i++) newArray[i] = array[i];
            array = newArray;
        }
        array[count++] = item;
    }

    public T Get(int index)
    {
        return array[index];
    }

    public void RemoveAt(int index)
    {
        for (int i = index; i < count - 1; i++) array[i] = array[i + 1];
        count--;
        array[count] = default(T);
    }
}