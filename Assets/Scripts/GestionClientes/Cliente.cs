using UnityEngine;
using TMPro;

public class Cliente : MonoBehaviour
{
    // Definimos los estados claramente separados
    public enum Estado { EsperandoAtencion, EsperandoComida, Satisfecho, Enojado }
    public Estado estadoActual = Estado.EsperandoAtencion; // Arranca esperando a que le tomes la orden

    public System.Collections.Generic.Queue<string> pedido = new System.Collections.Generic.Queue<string>();
    public TextMeshProUGUI textoPedido;

    private bool quiereComprar = true;
    private float tiempoRestante = 0f;
    private System.Action callbackSalida;
    private bool atendidoOFrustrado = false;
    private int cantidadIngredientesPedido = 0;

    public ClienteQueue miColaCliente;

    void Start()
    {
        if (tiempoRestante <= 0f && quiereComprar)
        {
            GenerarPedidoNormal();
        }
    }

    public void InicializarComportamiento(bool compra, float tiempoCompra, float tiempoNoCompra, System.Action alSalir)
    {
        quiereComprar = compra;
        callbackSalida = alSalir;
        atendidoOFrustrado = false;
        estadoActual = Estado.EsperandoAtencion; // Al spawnear, espera que le tomen la orden

        if (quiereComprar)
        {
            tiempoRestante = tiempoCompra; // 30 segundos totales de paciencia
            GenerarPedidoNormal();
        }
        else
        {
            tiempoRestante = tiempoNoCompra; // 5 segundos si solo mira
            DesactivarPedidoVisual();
        }
    }

    void Update()
    {
        if (atendidoOFrustrado) return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            atendidoOFrustrado = true;

            if (quiereComprar)
            {
                Debug.Log("¡Cliente cansado de esperar! Se fue sin ser atendido. Pierdes XP.");
                if (ManagerExperiencia.Instancia != null)
                {
                    ManagerExperiencia.Instancia.ModificarXP(-15);
                }
            }
            else
            {
                Debug.Log("El transeúnte se retiró en paz.");
            }

            DestruirCliente();
        }
    }

    private void GenerarPedidoNormal()
    {
        // Limpiamos el pedido por si acaso
        pedido.Clear();

        if (ManagerExperiencia.Instancia != null && ManagerExperiencia.Instancia.ingredientesDesbloqueados != null)
        {
            // Extraemos los ingredientes que el jugador YA tiene desbloqueados en este momento
            string[] ingredientesDisponibles = ManagerExperiencia.Instancia.ingredientesDesbloqueados.ToArray();

            if (ingredientesDisponibles.Length > 0)
            {
                // Decidimos cuántos ingredientes tendrá el shawarma (entre 1 y la cantidad disponible)
                int cantidadExtras = Random.Range(0, 3); // 0, 1 o 2 extras
                int totalDeseado = 1 + cantidadExtras;

                if (totalDeseado > ingredientesDisponibles.Length)
                    totalDeseado = ingredientesDisponibles.Length;

                // Añadimos ingredientes aleatorios de los que ya están desbloqueados
                while (pedido.Count < totalDeseado)
                {
                    string ingredienteAleatorio = ingredientesDisponibles[Random.Range(0, ingredientesDisponibles.Length)];

                    // Evitamos duplicar el mismo ingrediente en un mismo pedido si lo deseamos más limpio
                    // (O lo dejamos libre si querés que pueda pedir doble). Aquí lo encolamos directo:
                    pedido.Enqueue(ingredienteAleatorio);
                }
            }
        }

        ActualizarTextoPedido();
    }

    private void DesactivarPedidoVisual()
    {
        if (textoPedido != null)
        {
            textoPedido.text = "Solo mirando...";
        }
    }

    private void ActualizarTextoPedido()
    {
        if (textoPedido != null)
        {
            textoPedido.text = "Pedido: " + ObtenerTextoPedido();
        }
    }

    // --- NUEVO MÉTODO: El jugador toma la orden ---
    public void AceptarPedido()
    {
        if (estadoActual == Estado.EsperandoAtencion)
        {
            estadoActual = Estado.EsperandoComida; // Pasa al estado de esperar que cocines el shawarma
            Debug.Log("¡Orden tomada! Ahora prepara el shawarma.");
        }
    }

    // --- MÉTODOS REQUERIDOS POR LA COLA ---
    public void MoveTo(Vector3 nuevaPosicion)
    {
        transform.position = nuevaPosicion;
    }

    public string ObtenerTextoPedido()
    {
        if (pedido == null || pedido.Count == 0) return "Vacío";

        // Convertimos la cola temporalmente a un array para unirlos con " + "
        string[] itemsPedido = pedido.ToArray();
        string resultado = string.Join(" + ", itemsPedido);

        return resultado;
    }
    public void AtenderClienteExitoso()
    {
        if (!quiereComprar || atendidoOFrustrado || estadoActual != Estado.EsperandoComida) return;

        atendidoOFrustrado = true;

        // 1. Registramos la venta de dinero en el DayManager
        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarVenta(150f);
        }

        // 2. ¡Damos XP al jugador por hacer el pedido bien!
        if (ManagerExperiencia.Instancia != null)
        {
            ManagerExperiencia.Instancia.ModificarXP(25); // Suma 25 XP por venta exitosa
            Debug.Log("¡Shawarma entregado con éxito! +25 XP y dinero registrado.");
        }
        else
        {
            Debug.Log("¡Shawarma entregado con éxito!");
        }

        // 3. Limpiamos y removemos al cliente de la cola
        if (miColaCliente != null)
        {
            miColaCliente.DestroyClientAtendido(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void DestruirCliente()
    {
        if (miColaCliente != null)
        {
            miColaCliente.RemoverClienteDeCola(this);
        }
        Destroy(gameObject);
    }
}