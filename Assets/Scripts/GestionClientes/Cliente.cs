using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Cliente : MonoBehaviour, IPrototipo<Cliente>
{
    public enum Estado { EsperandoAtencion, EsperandoComida, Satisfecho, Enojado }
    public Estado estadoActual = Estado.EsperandoAtencion;

    public System.Collections.Generic.Queue<string> pedido = new System.Collections.Generic.Queue<string>();
    public TextMeshProUGUI textoPedido;

    private bool quiereComprar = true;
    private float tiempoRestante = 0f;
    private System.Action callbackSalida;
    private bool atendidoOFrustrado = false;
    private int cantidadIngredientesPedido = 0;
    
    // aca empieza el patron Prototype
    public Cliente Clonar(Vector3 posicion, Quaternion rotacion)
    {
        Cliente clon = Instantiate(this, posicion, rotacion);
        return clon;
    }

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
        estadoActual = Estado.EsperandoAtencion;

        if (quiereComprar)
        {
            tiempoRestante = tiempoCompra;
            GenerarPedidoNormal();
        }
        else
        {
            tiempoRestante = tiempoNoCompra;
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
        pedido.Clear();

        if (ManagerExperiencia.Instancia != null && ManagerExperiencia.Instancia.ingredientesDesbloqueados != null)
        {
            string[] ingredientesDisponibles = ManagerExperiencia.Instancia.ingredientesDesbloqueados.ToArray();

            if (ingredientesDisponibles.Length > 0)
            {
                int cantidadExtras = Random.Range(0, 3);
                int totalDeseado = 1 + cantidadExtras;

                if (totalDeseado > ingredientesDisponibles.Length)
                    totalDeseado = ingredientesDisponibles.Length;

                while (pedido.Count < totalDeseado)
                {
                    string ingredienteAleatorio = ingredientesDisponibles[Random.Range(0, ingredientesDisponibles.Length)];
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

    public void AceptarPedido()
    {
        if (estadoActual == Estado.EsperandoAtencion)
        {
            estadoActual = Estado.EsperandoComida;
            Debug.Log("¡Orden tomada! Ahora prepara el shawarma.");
        }
    }

    public void MoveTo(Vector3 nuevaPosicion)
    {
        transform.position = nuevaPosicion;
    }

    public string ObtenerTextoPedido()
    {
        if (pedido == null || pedido.Count == 0) return "Vacío";

        string[] itemsPedido = pedido.ToArray();
        string resultado = string.Join(" + ", itemsPedido);

        return resultado;
    }

    // NUEVO: Valida si los ingredientes del shawarma entregado coinciden con el pedido
    public void ValidarYAtenderShawarma(List<string> ingredientesShawarmaEntregado)
    {
        if (atendidoOFrustrado) return;
        atendidoOFrustrado = true;

        // Convertimos la Queue del pedido a una lista para comparar fácilmente
        List<string> listaPedidoCliente = new List<string>(pedido);

        // Verificamos si tienen la misma cantidad y los mismos ingredientes
        bool esCorrecto = true;

        if (ingredientesShawarmaEntregado == null || ingredientesShawarmaEntregado.Count != listaPedidoCliente.Count)
        {
            esCorrecto = false;
        }
        else
        {
            // Creamos copias temporales para comparar elementos
            List<string> copiaCliente = new List<string>(listaPedidoCliente);
            foreach (string ingrediente in ingredientesShawarmaEntregado)
            {
                if (copiaCliente.Contains(ingrediente))
                {
                    copiaCliente.Remove(ingrediente);
                }
                else
                {
                    esCorrecto = false;
                    break;
                }
            }
        }

        // Aplicamos recompensas o penalizaciones según el resultado
        if (esCorrecto)
        {
            int cantidadIngredientes = listaPedidoCliente.Count;
            if (ManagerExperiencia.Instancia != null)
            {
                ManagerExperiencia.Instancia.RegistrarVentaShawarma(cantidadIngredientes);
            }
            Debug.Log("¡Pedido correcto entregado con éxito!");
        }
        else
        {
            if (ManagerExperiencia.Instancia != null)
            {
                ManagerExperiencia.Instancia.RegistrarPedidoIncorrecto(10); // Resta 10 XP y no da dinero
            }
            Debug.Log("¡Pedido incorrecto! El cliente se va molesto.");
        }

        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarClienteAtendido();
        }

        // Retiramos al cliente de la cola y lo destruimos
        if (miColaCliente != null)
        {
            miColaCliente.DestroyClientAtendido(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método anterior adaptado por compatibilidad por si se llama directo
    public void AtenderClienteExitoso()
    {
        if (atendidoOFrustrado) return;
        atendidoOFrustrado = true;

        List<string> listaPedidoCliente = new List<string>(pedido);
        List<string> ingredientesEntregados = ManagerExperiencia.ingredientesShawarmaEnMano;

        bool esCorrecto = true;

        // 1. Validamos que tengan la misma cantidad exacta de ingredientes
        if (ingredientesEntregados == null || ingredientesEntregados.Count != listaPedidoCliente.Count)
        {
            esCorrecto = false;
        }
        else
        {
            // 2. Validamos que contengan exactamente los mismos elementos (sin importar el orden)
            List<string> copiaCliente = new List<string>(listaPedidoCliente);
            foreach (string ingrediente in ingredientesEntregados)
            {
                if (copiaCliente.Contains(ingrediente))
                {
                    copiaCliente.Remove(ingrediente);
                }
                else
                {
                    esCorrecto = false;
                    break;
                }
            }
        }

        // 3. Otorgamos recompensas o penalizaciones según el resultado estricto
        if (esCorrecto)
        {
            int cantidadIngredientes = listaPedidoCliente.Count;
            if (ManagerExperiencia.Instancia != null)
            {
                ManagerExperiencia.Instancia.RegistrarVentaShawarma(cantidadIngredientes);
            }
            Debug.Log("¡Pedido 100% correcto entregado con éxito!");
        }
        else
        {
            if (ManagerExperiencia.Instancia != null)
            {
                ManagerExperiencia.Instancia.RegistrarPedidoIncorrecto(10); // Resta 10 XP y 0 dinero
            }
            Debug.Log("¡Pedido incorrecto o incompleto! Se penaliza al jugador.");
        }

        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarClienteAtendido();
        }

        // Limpiamos la mano
        ManagerExperiencia.ingredientesShawarmaEnMano.Clear();

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