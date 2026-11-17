using UnityEngine;

public class ClienteSpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject prefabCliente; 
    public Transform puntoSpawn;     
    public float intervaloSpawn = 40f; 

    [Header("Tiempos de Comportamiento")]
    public float tiempoEsperaComprador = 30f; 
    public float tiempoEsperaNoComprador = 5f; 
    [Range(0f, 1f)] public float probabilidadCompra = 0.7f; 

    DayObserver dayObserver = new DayObserver();
    HandObserver handObserver = new HandObserver();
    XPObserver xpObserver = new XPObserver();

    private Cliente clienteActualEnSpawn = null;
    private float temporizadorSpawn = 0f;

    void Update()
    {
        if (clienteActualEnSpawn != null) return;

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

    public void OnClienteSeFue()
    {
        clienteActualEnSpawn = null;
        temporizadorSpawn = 0f;
    }
}