using Unity.VisualScripting;
using UnityEngine;

public class ClienteQueue : MonoBehaviour
{
    [SerializeField] Cliente clientPrefab;
    [SerializeField] private Transform spawnPoint;
    private float distance = 35f;

    SimpleLinkedQueue<Cliente> clientQueue = new SimpleLinkedQueue<Cliente>();

    private void Update()
    {
        //CON LA H SE SPAWNEAN CLIENTES
        if (Input.GetKeyDown(KeyCode.H)) 
        {
            CreateClient();
        }
        //CON LA J SE DEQUEUEAN
        if (Input.GetKeyDown(KeyCode.J))
        {
            DestroyClient();
        }
    }

    public void CreateClient()
    {
        Cliente newClient = Instantiate(clientPrefab, (spawnPoint.position), Quaternion.identity);
        clientQueue.Enqueue(newClient);
        UpdatePosition(newClient);
    }
    
    public void DestroyClient()
    {
        Cliente clientToDestroy = clientQueue.Dequeue();
        Destroy(clientToDestroy.gameObject);
        Cliente nextClient = clientQueue.Peek();
        nextClient.MoveTo(spawnPoint.position);

    }
    public void UpdatePosition(Cliente client)
    {
        if (client != null)
        {
            Cliente clientToUpdate = client;
            clientToUpdate.gameObject.transform.position = spawnPoint.position + Vector3.forward * (clientQueue.Count - 1) * distance;
        }
    }
}
