public class DictionaryNode<TKey, TValue>
{
    public TKey key;
    public TValue value;

    public DictionaryNode<TKey, TValue> next;

    public DictionaryNode(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;

        next = null;
    }
}