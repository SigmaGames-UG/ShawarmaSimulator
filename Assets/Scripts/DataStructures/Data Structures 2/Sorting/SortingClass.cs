using System;

public static class CollectionSort
{
    public static void SortArray<T>(T[] array)
        where T : IComparable<T>
    {
        SortArray(array, (left, right) => left.CompareTo(right));
    }

    public static void SortArray<T>(T[] array, Comparison<T> compare)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - 1 - i; j++)
            {
                if (compare(array[j], array[j + 1]) > 0)
                {
                    T temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }
}
