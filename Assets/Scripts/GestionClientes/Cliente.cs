using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Cliente : MonoBehaviour, IPrototipo<Cliente>
{
    public event System.Action<Cliente> EntregaExitosa;
    public event System.Action<Cliente> EntregaFallida;
    public event System.Action<Cliente> ClienteSeFue;

    public enum Estado { EsperandoAtencion, EsperandoComida, Satisfecho, Enojado }
    public Estado estadoActual = Estado.EsperandoAtencion;

    public System.Collections.Generic.Queue<string> pedido = new System.Collections.Generic.Queue<string>();
    public TextMeshProUGUI textoPedido;
    public ClienteQueue miColaCliente;

    public int CantidadIngredientes => pedido.Count;

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

    public void InicializarComportamiento(bool compra, float tiempoCompra, float tiempoNoCompra, System.Action alSalir)
    {
        quiereComprar = compra;
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
            if (textoPedido != null) textoPedido.text = "Solo mirando...";
        }
    }

    void Update()
    {
        if (atendidoOFrustrado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante > 0f) return;

        atendidoOFrustrado = true;

        if (quiereComprar)
            ClienteSeFue?.Invoke(this);

        Salir(atendido: false);
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

    
    public void ValidarYAtenderShawarma(List<string> ingredientesShawarmaEntregado)
    {
        if (atendidoOFrustrado) return;
        atendidoOFrustrado = true;

        if (EsPedidoCorrecto(ingredientesShawarmaEntregado))
            EntregaExitosa?.Invoke(this);
        else
            EntregaFallida?.Invoke(this);

        Salir(atendido: true);
    }
    
    private bool EsPedidoCorrecto(List<string> entregados)
    {
        if (entregados == null || entregados.Count != pedido.Count) return false;

        List<string> copia = new List<string>(pedido);
        foreach (string ingrediente in entregados)
        {
            if (!copia.Remove(ingrediente)) return false;
        }
        return true;
    }

    private void Salir(bool atendido)
    {
        if (miColaCliente != null)
        {
            if (atendido) miColaCliente.DestroyClientAtendido(this);
            else miColaCliente.RemoverClienteDeCola(this);
        }
        Destroy(gameObject);
    }
}