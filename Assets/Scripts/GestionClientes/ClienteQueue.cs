using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ClienteQueue : MonoBehaviour
{
    public static ClienteQueue Instancia;

    [Header("Lista de Prefabs de Clientes")]
    [Tooltip("Arrastra aquí varios prefabs de personajes diferentes.")]
    [SerializeField] private List<Cliente> listaPrefabsClientes = new List<Cliente>();

    [SerializeField] private Transform spawnPoint;

    [Header("Configuración de Rotación del Modelo")]
    [SerializeField] private Vector3 rotacionExtraOffset = new Vector3(0f, 0f, 90f);

    [Header("Configuración de la Fila y Spawn")]
    public int maxClientesEnCola = 3;        
    public float separacionFila = 2.5f;      
    public float intervaloSpawn = 20f;       
    [Range(0f, 1f)] public float probabilidadCompra = 0.7f;
    public float tiempoEsperaComprador = 30f;
    public float tiempoEsperaNoComprador = 5f;

    private float temporizadorSpawn = 0f;

    private SimpleLinkedQueue<Cliente> clientQueue = new SimpleLinkedQueue<Cliente>();
    private List<Cliente> listaClientesEnEspera = new List<Cliente>();

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoPedidosUI;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (EsLocalAbierto())
        {
            if (listaClientesEnEspera.Count < maxClientesEnCola)
            {
                temporizadorSpawn += Time.deltaTime;
                if (temporizadorSpawn >= intervaloSpawn)
                {
                    temporizadorSpawn = 0f;
                    CreateClientAutomatico();
                }
            }
        }

        ActualizarPantallaPedidos();
    }

    bool EsLocalAbierto()
    {
        if (DayManager.Instancia != null)
        {
            return DayManager.Instancia.EstaAbierto;
        }
        return true;
    }

    public void CreateClientAutomatico()
    {
        if (listaPrefabsClientes == null || listaPrefabsClientes.Count == 0 || spawnPoint == null) return;

        Cliente prefabAleatorio = listaPrefabsClientes[Random.Range(0, listaPrefabsClientes.Count)];
        if (prefabAleatorio == null) return;

        int indexEnFila = listaClientesEnEspera.Count;

        Vector3 direccionFila = -spawnPoint.forward;
        direccionFila.y = 0f;
        direccionFila.Normalize();

        Vector3 posicionEnFila = spawnPoint.position + (direccionFila * (indexEnFila * separacionFila));
        posicionEnFila.y = spawnPoint.position.y;

        Quaternion rotacionFinal = spawnPoint.rotation * Quaternion.Euler(rotacionExtraOffset);

        IPrototipo<Cliente> prototipo = prefabAleatorio;

        Cliente newClient = prototipo.Clonar(posicionEnFila, rotacionFinal);

        clientQueue.Enqueue(newClient);
        listaClientesEnEspera.Add(newClient);

        newClient.miColaCliente = this;

        bool quiereComprar = Random.value <= probabilidadCompra;
        newClient.InicializarComportamiento(quiereComprar, tiempoEsperaComprador, tiempoEsperaNoComprador, () => {
            RemoverClienteDeCola(newClient);
        });

        Debug.Log("Cliente automático añadido a la posición " + indexEnFila + " de la fila.");
    }

    public void RemoverClienteDeCola(Cliente clienteAEliminar)
    {
        if (listaClientesEnEspera.Contains(clienteAEliminar))
        {
            listaClientesEnEspera.Remove(clienteAEliminar);
        }

        int n = clientQueue.Count;
        SimpleLinkedQueue<Cliente> colaTemporal = new SimpleLinkedQueue<Cliente>();

        for (int i = 0; i < n; i++)
        {
            Cliente c = clientQueue.Dequeue();
            if (c != null && c != clienteAEliminar)
            {
                colaTemporal.Enqueue(c);
            }
        }
        clientQueue = colaTemporal;

        ActualizarTodasLasPosiciones();
    }

    public void DestroyClientAtendido(Cliente clienteAtendido)
    {
        RemoverClienteDeCola(clienteAtendido);
        if (clienteAtendido != null)
        {
            Destroy(clienteAtendido.gameObject);
        }
    }

    public void ActualizarTodasLasPosiciones()
    {
        Vector3 direccionFila = -spawnPoint.forward;
        direccionFila.y = 0f;
        direccionFila.Normalize();

        for (int i = 0; i < listaClientesEnEspera.Count; i++)
        {
            Cliente cliente = listaClientesEnEspera[i];
            if (cliente != null)
            {
                Vector3 nuevaPosicion = spawnPoint.position + (direccionFila * (i * separacionFila));
                nuevaPosicion.y = spawnPoint.position.y;
                cliente.transform.position = nuevaPosicion;
            }
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

        if (ordenesPendientes == 0)
        {
            textoFinal += "Sin pedidos por ahora...";
        }

        textoPedidosUI.text = textoFinal;
    }
}