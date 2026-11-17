using UnityEngine;

public interface IGraph
{
    public interface IGraph<T>
    {
        T[] Vertices { get; }

        bool AddVertex(T vertex);
        bool RemoveVertex(T vertex);
        bool ContainsVertex(T vertex);

        bool AddEdge(T from, T to, int weight);
        bool RemoveEdge(T from, T to);
        bool ContainsEdge(T from, T to);
        int GetWeight(T from, T to);

        ISimpleList<Edge<T>> GetNeighbors(T vertex);
    }
}
