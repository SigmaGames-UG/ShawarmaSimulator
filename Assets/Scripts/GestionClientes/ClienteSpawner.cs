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

    private GameObject clienteActualEnSpawn = null;
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

        // Instanciamos al cliente en el punto de spawn
        clienteActualEnSpawn = Instantiate(prefabCliente, puntoSpawn.position, puntoSpawn.rotation);

        // Determinamos de forma aleatoria si decide comprar o no según la probabilidad
        bool quiereComprar = UnityEngine.Random.value <= probabilidadCompra;

        // Configuramos su comportamiento inteligente (le pasamos los datos al script del cliente)
        Cliente InteligenciaCliente = clienteActualEnSpawn.GetComponent<Cliente>();
        if (InteligenciaCliente != null)
        {
            InteligenciaCliente.InicializarComportamiento(quiereComprar, tiempoEsperaComprador, tiempoEsperaNoComprador, OnClienteSeFue);
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