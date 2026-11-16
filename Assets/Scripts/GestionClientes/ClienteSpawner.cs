using UnityEngine;

public class ClienteSpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject prefabCliente; // El prefab de tu cliente
    public Transform puntoSpawn;     // Donde aparece el cliente
    public float intervaloSpawn = 40f; // Cada cuánto intenta spawnear (40 segundos)

    [Header("Tiempos de Comportamiento")]
    public float tiempoEsperaComprador = 30f; // Tiempo límite si decide comprar
    public float tiempoEsperaNoComprador = 5f; // Tiempo que se queda si decide no comprar
    [Range(0f, 1f)] public float probabilidadCompra = 0.7f; // 70% de probabilidad de que quiera comprar

    DayObserver dayObserver = new DayObserver();
    HandObserver handObserver = new HandObserver();
    XPObserver xpObserver = new XPObserver();

    private Cliente clienteActualEnSpawn = null;
    private float temporizadorSpawn = 0f;

    void Update()
    {
        // Si ya hay un cliente en el spawn, no dejamos aparecer a otro
        if (clienteActualEnSpawn != null) return;

        // Controlamos el temporizador de 40 segundos
        temporizadorSpawn += Time.deltaTime;
        if (temporizadorSpawn >= intervaloSpawn)
        {
            temporizadorSpawn = 0f;
            GenerarClienteInteligente();
        }
    }

    void GenerarClienteInteligente()
    {
        if (prefabCliente == null || puntoSpawn == null) return;


        
        Cliente nuevo = clienteActualEnSpawn.Clonar(puntoSpawn.position, puntoSpawn.rotation);

        handObserver.Suscribir(nuevo);
        dayObserver.Suscribir(nuevo);
        xpObserver.Suscribir(nuevo);

        bool quiereComprar = UnityEngine.Random.value <= probabilidadCompra;

        
        if (nuevo != null)
        {
            nuevo.InicializarComportamiento(quiereComprar, tiempoEsperaComprador, tiempoEsperaNoComprador, OnClienteSeFue);
        }

        Debug.Log(quiereComprar ? "Cliente llegó con intención de comprar." : "Cliente llegó solo de paso (no comprará).");
    }

    // Callback para limpiar la referencia cuando el cliente se va solo o es atendido
    public void OnClienteSeFue()
    {
        clienteActualEnSpawn = null;
        // Reiniciamos el conteo para los próximos 40 segundos desde que se libera el espacio
        temporizadorSpawn = 0f;
    }
}