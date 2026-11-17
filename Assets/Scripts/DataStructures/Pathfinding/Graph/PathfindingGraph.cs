using UnityEngine;

public class PathfindingGraph : MonoBehaviour
{
    public Graph<Transform> Graph { get; private set; }
    public Transform Spawn { get; private set; }
    public Transform Order { get; private set; }
    public Transform Chair1 { get; private set; }
    public Transform Chair2 { get; private set; }
    public Transform Chair3 { get; private set; }

    private void Awake()
    {
        Spawn = transform.Find("Spawn");
        Order = transform.Find("Order");
        Chair1 = transform.Find("Chair1");
        Chair2 = transform.Find("Chair2");
        Chair3 = transform.Find("Chair3");

        if (Spawn == null || Order == null || Chair1 == null || Chair2 == null || Chair3 == null)
        {
            Debug.Log("uno de los transforms es null");
            return;
        }

        Graph = new Graph<Transform>(
            new SimpleLinkedDictionary<Transform, ISimpleList<Edge<Transform>>>(),
            () => new SimpleLinkedList<Edge<Transform>>());

        Graph.AddVertex(Spawn);
        Graph.AddVertex(Order);
        Graph.AddVertex(Chair1);
        Graph.AddVertex(Chair2);
        Graph.AddVertex(Chair3);

        Graph.AddEdge(Spawn, Order, 3);
        Graph.AddEdge(Order, Chair1, 4);
        Graph.AddEdge(Order, Chair2, 5);
        Graph.AddEdge(Order, Chair3, 6);
        
    }
    [SerializeField] private ClientePathMovement testClient;

    private void Start()
    {
        if (Graph == null || testClient == null)
            return;

        testClient.transform.position = Spawn.position;

        if (!testClient.TryMoveTo(Graph, Spawn, Chair2))
            Debug.LogWarning("No route from Spawn to Chair2.", this);
    }
}