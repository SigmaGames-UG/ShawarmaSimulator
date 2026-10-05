using System;
using System.Collections.Generic;

public class SimpleLinkedDictionary<TKey, TValue>
    : ISimpleDictionary<TKey, TValue>
{
    private DictionaryNode<TKey, TValue> first;
    private DictionaryNode<TKey, TValue> last;

    private int count;

    public int Count
    {
        get { return count; }
    }

    public bool IsEmpty
    {
        get { return count == 0; }
    }

    public SimpleLinkedDictionary()
    {
        first = null;
        last = null;
        count = 0;
    }

    public TValue this[TKey key]
    {
        get
        {
            ValidateKey(key);

            DictionaryNode<TKey, TValue> node =
                GetNodeByKey(key);

            if (node == null)
                throw new KeyNotFoundException();

            return node.value;
        }

        set
        {
            ValidateKey(key);

            DictionaryNode<TKey, TValue> node =
                GetNodeByKey(key);

            if (node == null)
            {
                ExecuteAdd(key, value);
            }
            else
            {
                node.value = value;
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        ValidateKey(key);

        if (ContainsKey(key))
        {
            throw new ArgumentException(
                "La key ya existe en el Dictionary."
            );
        }

        ExecuteAdd(key, value);
    }

    public bool TryAdd(TKey key, TValue value)
    {
        ValidateKey(key);

        if (ContainsKey(key))
            return false;

        ExecuteAdd(key, value);

        return true;
    }

    public bool ContainsKey(TKey key)
    {
        ValidateKey(key);

        return GetNodeByKey(key) != null;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        ValidateKey(key);

        DictionaryNode<TKey, TValue> node =
            GetNodeByKey(key);

        if (node != null)
        {
            value = node.value;
            return true;
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        ValidateKey(key);

        if (first == null)
            return false;

        // borrar el primero.
        if (EqualityComparer<TKey>.Default.Equals(
            first.key,
            key))
        {
            first = first.next;

            count--;

            if (first == null)
            {
                last = null;
            }

            return true;
        }

        DictionaryNode<TKey, TValue> current = first;

        while (current.next != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(
                current.next.key,
                key))
            {
                if (current.next == last)
                {
                    last = current;
                }

                current.next = current.next.next;

                count--;

                return true;
            }

            current = current.next;
        }

        return false;
    }

    public void Clear()
    {
        first = null;
        last = null;
        count = 0;
    }

    public TKey[] Keys()
    {
        TKey[] keys = new TKey[count];

        DictionaryNode<TKey, TValue> current = first;

        int index = 0;

        while (current != null)
        {
            keys[index] = current.key;

            index++;
            current = current.next;
        }

        return keys;
    }

    public TValue[] Values()
    {
        TValue[] values = new TValue[count];

        DictionaryNode<TKey, TValue> current = first;

        int index = 0;

        while (current != null)
        {
            values[index] = current.value;

            index++;
            current = current.next;
        }

        return values;
    }

    private DictionaryNode<TKey, TValue> GetNodeByKey(
        TKey key)
    {
        DictionaryNode<TKey, TValue> current = first;

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(
                current.key,
                key))
            {
                return current;
            }

            current = current.next;
        }

        return null;
    }

    private void ExecuteAdd(TKey key, TValue value)
    {
        DictionaryNode<TKey, TValue> nuevo =
            new DictionaryNode<TKey, TValue>(
                key,
                value
            );

        if (first == null)
        {
            first = nuevo;
            last = nuevo;
        }
        else
        {
            last.next = nuevo;
            last = nuevo;
        }

        count++;
    }

    private void ValidateKey(TKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }
    }
}