using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ClienteQueue : MonoBehaviour
{
    public static ClienteQueue Instancia;

    [Header("Lista de Prefabs de Clientes")]
    [Tooltip("Arrastra aquí varios prefabs de personajes diferentes.")]
    [SerializeField] private List<Cliente> listaPrefabsClientes = new List<Cliente>();

    [SerializeField] private PathfindingGraph pathfinding;
    private readonly HashSet<Transform> sillasReservadas = new();

    [SerializeField] private Transform spawnPoint;

    DayObserver dayObserver = new DayObserver();
    HandObserver handObserver = new HandObserver();
    XPObserver xpObserver = new XPObserver();

    [Header("Configuración de Rotación del Modelo")]
    [SerializeField] private Vector3 rotacionExtraOffset = new Vector3(0f, 0f, 90f);

    [Header("Configuración de la Fila y Spawn")]
    public int maxClientesEnCola = 3;
    public float separacionFila = 2.5f;
    public float intervaloSpawn = 10f;
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

    public bool TryEnviarASilla(Cliente cliente, out Transform sillaReservada)
    {
        sillaReservada = null;

        if (pathfinding == null || pathfinding.Graph == null
            || !cliente.TryGetComponent(out ClientePathMovement movement))
            return false;

        Transform[] sillas = { pathfinding.Chair1, pathfinding.Chair2, pathfinding.Chair3 };

        foreach (Transform silla in sillas)
        {
            if (silla == null || !sillasReservadas.Add(silla))
                continue;

            if (movement.TryMoveTo(pathfinding.Graph, pathfinding.Order, silla, cliente.AlLlegarASilla))
            {
                sillaReservada = silla;
                RemoverClienteDeCola(cliente);
                return true;
                
            }

            sillasReservadas.Remove(silla);
        }

        return false;
    }

    public void LiberarSilla(Transform silla)
    {
        sillasReservadas.Remove(silla);
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

        Vector3 posicionEnFila = ObtenerPosicionEnFila(indexEnFila);

        Quaternion rotacionFinal = spawnPoint.rotation * Quaternion.Euler(rotacionExtraOffset);

        Cliente newClient = ClientePool.Instancia.ObtenerCliente(prefabAleatorio, posicionEnFila, rotacionFinal);

        handObserver.Suscribir(newClient);
        dayObserver.Suscribir(newClient);
        xpObserver.Suscribir(newClient);

        clientQueue.Enqueue(newClient);
        listaClientesEnEspera.Add(newClient);

        newClient.miColaCliente = this;

        bool quiereComprar = Random.value <= probabilidadCompra;
        newClient.InicializarComportamiento(quiereComprar, tiempoEsperaComprador, tiempoEsperaNoComprador, () => {
            RemoverClienteDeCola(newClient);
        });

        if (indexEnFila == 0)
            EnviarPrimeroAlPedido();
        else
            newClient.EsperarEnFila();

        Debug.Log("Cliente automático añadido a la posición " + indexEnFila + " de la fila.");
    }

    public void RemoverClienteDeCola(Cliente clienteAEliminar)
    {
        int indice = listaClientesEnEspera.IndexOf(clienteAEliminar);
        if (indice < 0) return;

        listaClientesEnEspera.RemoveAt(indice);
        handObserver.Desuscribir(clienteAEliminar);
        dayObserver.Desuscribir(clienteAEliminar);
        xpObserver.Desuscribir(clienteAEliminar);

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

        if (indice == 0)
            EnviarPrimeroAlPedido();

        ActualizarTodasLasPosiciones();
    }

    public bool EsPrimeroEnFila(Cliente cliente)
    {
        return !clientQueue.IsEmpty && clientQueue.Peek() == cliente;
    }

    private void EnviarPrimeroAlPedido()
    {
        if (clientQueue.IsEmpty) return;

        Cliente primero = clientQueue.Peek();
        if (primero == null) return;

        // A newly promoted customer enters the graph at the spawn node.
        primero.MoveTo(ObtenerPosicionEnFila(0));
        if (pathfinding != null && pathfinding.Graph != null
            && primero.TryGetComponent(out ClientePathMovement movement)
            && movement.TryMoveTo(pathfinding.Graph, pathfinding.Spawn, pathfinding.Order, primero.AlLlegarAlPedido))
        {
            primero.CaminarAlPedido();
        }
        else
        {
            primero.EsperarEnFila();
        }
    }

    private Vector3 ObtenerPosicionEnFila(int indice)
    {
        Vector3 direccionFila = -spawnPoint.forward;
        direccionFila.y = 0f;
        return spawnPoint.position + direccionFila.normalized * (indice * separacionFila);
    }

    public void ActualizarTodasLasPosiciones()
    {
        // The first customer owns the counter and may already be walking or waiting there.
        for (int i = 1; i < listaClientesEnEspera.Count; i++)
        {
            Cliente cliente = listaClientesEnEspera[i];
            if (cliente != null)
                cliente.MoveTo(ObtenerPosicionEnFila(i));
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
