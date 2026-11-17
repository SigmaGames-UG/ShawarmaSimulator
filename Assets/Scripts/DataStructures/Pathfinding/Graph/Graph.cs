using System;
using System.Collections.Generic;
using static IGraph;

public class Graph<T> : IGraph<T>
{
    private readonly ISimpleDictionary<T, ISimpleList<Edge<T>>> adjacency;
    private readonly Func<ISimpleList<Edge<T>>> createEdgeList;

    public Graph(
        ISimpleDictionary<T, ISimpleList<Edge<T>>> adjacency,
        Func<ISimpleList<Edge<T>>> createEdgeList)
    {
        this.adjacency = adjacency;
        this.createEdgeList = createEdgeList;
    }

    public T[] Vertices => adjacency.Keys();

    public bool AddVertex(T vertex)
    {
        if (ContainsVertex(vertex))
            return false;

        return adjacency.TryAdd(vertex, createEdgeList());
    }

    public bool ContainsVertex(T vertex)
    {
        return adjacency.ContainsKey(vertex);
    }

    public ISimpleList<Edge<T>> GetNeighbors(T vertex)
    {
        return adjacency[vertex];
    }

    public bool AddEdge(T from, T to, int weight)
    {
        if (weight < 0)
            throw new ArgumentOutOfRangeException(nameof(weight));

        if (!adjacency.TryGetValue(from, out var edges) || !ContainsVertex(to))
            return false;

        int index = FindEdgeIndex(edges, to);

        if (index >= 0)
        {
            if (edges.Get(index).Weight == weight)
                return false;

            edges.RemoveAt(index);
        }

        edges.Add(new Edge<T>(to, weight));
        return true;
    }

    public bool ContainsEdge(T from, T to)
    {
        return adjacency.TryGetValue(from, out var edges)
            && FindEdgeIndex(edges, to) >= 0;
    }

    public int GetWeight(T from, T to)
    {
        if (adjacency.TryGetValue(from, out var edges))
        {
            int index = FindEdgeIndex(edges, to);

            if (index >= 0)
                return edges.Get(index).Weight;
        }

        throw new InvalidOperationException("The requested edge does not exist.");
    }

    public bool RemoveEdge(T from, T to)
    {
        if (!adjacency.TryGetValue(from, out var edges))
            return false;

        int index = FindEdgeIndex(edges, to);

        if (index < 0)
            return false;

        edges.RemoveAt(index);
        return true;
    }

    public bool RemoveVertex(T vertex)
    {
        if (!adjacency.Remove(vertex))
            return false;

        foreach (T remainingVertex in Vertices)
        {
            RemoveEdge(remainingVertex, vertex);
        }

        return true;
    }

    private static int FindEdgeIndex(ISimpleList<Edge<T>> edges, T destination)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            Edge<T> edge = edges.Get(i);

            if (EqualityComparer<T>.Default.Equals(edge.Destination, destination))
                return i;
        }

        return -1;
    }
}