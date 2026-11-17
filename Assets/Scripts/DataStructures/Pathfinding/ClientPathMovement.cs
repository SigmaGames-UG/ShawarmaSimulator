using UnityEngine;

public class ClientePathMovement : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float speed = 10f;

    private Transform[] path;
    private int nextWaypoint;
    private System.Action onArrival;


    public bool TryMoveTo(
        IGraph.IGraph<Transform> graph, Transform start, Transform destination, System.Action onArrival = null)
    {
        if (!Dijkstra.TryFindPath(graph, start, destination, out Transform[] route))
            return false;

        path = route;
        nextWaypoint = 1;
        this.onArrival = onArrival;
        return true;
    }

    private void Update()
    {
        if (path == null || nextWaypoint >= path.Length)
            return;

        Vector3 target = path[nextWaypoint].position;
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        transform.position = Vector3.MoveTowards(
            transform.position, target, speed * Time.deltaTime);

        if (transform.position.Equals(target))
        {
            nextWaypoint++;

            if (nextWaypoint == path.Length)
            {
                path = null;
                System.Action callback = onArrival;
                onArrival = null;
                callback?.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        path = null;
        onArrival = null;
    }
}