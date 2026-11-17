using System.Collections.Generic;

public class Edge<T>
{
    
    public T Destination { get; }
    public int Weight { get; }

    public Edge(T destination, int weight)
    {
        Destination = destination;
        Weight = weight;
    }

    public override bool Equals(object obj)
    {
        return obj is Edge<T> other
            && EqualityComparer<T>.Default.Equals(Destination, other.Destination)
            && Weight == other.Weight;
    }

    public override int GetHashCode()
    {
        return (Destination, Weight).GetHashCode();
    }
}