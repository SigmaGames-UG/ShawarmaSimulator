using TMPro;
using UnityEngine;

public class ClienteQueue : MonoBehaviour
{
    [SerializeField] Cliente clientPrefab;
    [SerializeField] private Transform spawnPoint;
    private float distance = 35f;

    SimpleLinkedQueue<Cliente> clientQueue = new SimpleLinkedQueue<Cliente>();

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoPedidosUI;

    private void Update()
    {
        // CON LA H SE SPAWNEAN CLIENTES
        if (Input.GetKeyDown(KeyCode.H))
        {
            CreateClient();
        }
        // CON LA J SE DEQUEUEAN
        if (Input.GetKeyDown(KeyCode.J))
        {
            DestroyClient();
        }

        ActualizarPantallaPedidos();
    }

    public void CreateClient()
    {
        Cliente newClient = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
        clientQueue.Enqueue(newClient);
        newClient.clienteQueue = this;
        UpdatePosition(newClient);
    }

    public void DestroyClient()
    {
        Cliente clientToDestroy = clientQueue.Dequeue();
        if (clientToDestroy != null)
        {
            Destroy(clientToDestroy.gameObject);
        }

        Cliente nextClient = clientQueue.Peek();
        if (nextClient != null)
        {
            nextClient.MoveTo(spawnPoint.position);
        }
    }

    public void UpdatePosition(Cliente client)
    {
        if (client != null)
        {
            client.gameObject.transform.position = spawnPoint.position + Vector3.forward * (clientQueue.Count - 1) * distance;
        }
    }

    public void ActualizarPantallaPedidos()
    {
        if (textoPedidosUI == null) return;

        string textoFinal = "ÓRDENES ACTIVAS:\n\n";
        int ordenesPendientes = 0;

        int n = clientQueue.Count;

        for (int i = 0; i < n; i++)
        {
            Cliente cliente = clientQueue.Dequeue();

            if (cliente != null && cliente.estadoActual == Cliente.Estado.EsperandoComida)
            {
                textoFinal += "- Shawarma (" + cliente.ObtenerTextoPedido() + ")\n";
                ordenesPendientes++;
            }

           
            clientQueue.Enqueue(cliente);
        }

        if (ordenesPendientes == 0) textoFinal += "Sin pedidos por ahora...";

        textoPedidosUI.text = textoFinal;
    }
}