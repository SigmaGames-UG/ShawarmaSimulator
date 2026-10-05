using System;
using System.Collections.Generic;

public class SimpleArrayDictionary<TKey, TValue>
    : ISimpleDictionary<TKey, TValue>
{
    private KeyValuePair<TKey, TValue>[] items;
    private int count;

    public int Count
    {
        get { return count; }
    }

    public bool IsEmpty
    {
        get { return count == 0; }
    }

    public SimpleArrayDictionary(int capacity = 4)
    {
        if (capacity < 1)
            capacity = 1;

        items = new KeyValuePair<TKey, TValue>[capacity];
        count = 0;
    }

    public TValue this[TKey key]
    {
        get
        {
            ValidateKey(key);

            int index = IndexOf(key);

            if (index == -1)
                throw new KeyNotFoundException();

            return items[index].Value;
        }

        set
        {
            ValidateKey(key);

            int index = IndexOf(key);
        
            // Si no existe
            if (index == -1)
            {
                ExecuteAdd(key, value);
            }


            // Si existe
            else
            {
                items[index] =
                    new KeyValuePair<TKey, TValue>(key, value);
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        ValidateKey(key);

        if (ContainsKey(key))
            throw new ArgumentException(
                "La key ya existe en el Dictionary."
            );

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

        return IndexOf(key) != -1;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        ValidateKey(key);

        int index = IndexOf(key);

        if (index != -1)
        {
            value = items[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        ValidateKey(key);

        int index = IndexOf(key);

        if (index == -1)
            return false;

        int ultimoIndex = count - 1;

       
        items[index] = items[ultimoIndex];

        items[ultimoIndex] = default;

        count--;

        return true;
    }

    public void Clear()
    {
        for (int i = 0; i < count; i++)
        {
            items[i] = default;
        }

        count = 0;
    }

    public TKey[] Keys()
    {
        TKey[] keys = new TKey[count];

        for (int i = 0; i < count; i++)
        {
            keys[i] = items[i].Key;
        }

        return keys;
    }

    public TValue[] Values()
    {
        TValue[] values = new TValue[count];

        for (int i = 0; i < count; i++)
        {
            values[i] = items[i].Value;
        }

        return values;
    }

    private int IndexOf(TKey key)
    {
        for (int i = 0; i < count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(
                items[i].Key,
                key))
            {
                return i;
            }
        }

        return -1;
    }

    private void ExecuteAdd(TKey key, TValue value)
    {
        ValidateSize();

        items[count] =
            new KeyValuePair<TKey, TValue>(key, value);

        count++;
    }

    private void ValidateSize()
    {
        if (count >= items.Length)
        {
            Resize(items.Length * 2);
        }
    }

    private void Resize(int newSize)
    {
        KeyValuePair<TKey, TValue>[] nuevoArray =
            new KeyValuePair<TKey, TValue>[newSize];

        for (int i = 0; i < count; i++)
        {
            nuevoArray[i] = items[i];
        }

        items = nuevoArray;
    }

    private void ValidateKey(TKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }
    }
}