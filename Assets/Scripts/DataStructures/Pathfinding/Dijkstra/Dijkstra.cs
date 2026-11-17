using System;

public static class Dijkstra
{
    public static bool TryFindPath<T>(
        IGraph.IGraph<T> graph, T start, T goal, out T[] path)
    {
        path = Array.Empty<T>();

        T[] vertices = graph.Vertices;
        int startIndex = Array.IndexOf(vertices, start);
        int goalIndex = Array.IndexOf(vertices, goal);

        if (startIndex < 0 || goalIndex < 0)
            return false;

        long[] cost = new long[vertices.Length];
        int[] previous = new int[vertices.Length];
        bool[] settled = new bool[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            cost[i] = long.MaxValue;
            previous[i] = -1;
        }

        cost[startIndex] = 0;

        while (true)
        {
            int current = -1;
            long cheapest = long.MaxValue;

            for (int i = 0; i < vertices.Length; i++)
            {
                if (!settled[i] && cost[i] < cheapest)
                {
                    current = i;
                    cheapest = cost[i];
                }
            }

            if (current == -1)
                return false;

            settled[current] = true;

            if (current == goalIndex)
                break;

            ISimpleList<Edge<T>> edges = graph.GetNeighbors(vertices[current]);

            for (int i = 0; i < edges.Count; i++)
            {
                Edge<T> edge = edges.Get(i);
                int neighbor = Array.IndexOf(vertices, edge.Destination);

                if (neighbor < 0 || settled[neighbor])
                    continue;

                long candidate = cost[current] + edge.Weight;

                if (candidate < cost[neighbor])
                {
                    cost[neighbor] = candidate;
                    previous[neighbor] = current;
                }
            }
        }

        
        int length = 1;
        for (int step = goalIndex; step != startIndex; step = previous[step])
            length++;

        path = new T[length];
        int vertexIndex = goalIndex;

        for (int i = length - 1; i >= 0; i--)
        {
            path[i] = vertices[vertexIndex];
            vertexIndex = previous[vertexIndex];
        }

        return true;
    }
}