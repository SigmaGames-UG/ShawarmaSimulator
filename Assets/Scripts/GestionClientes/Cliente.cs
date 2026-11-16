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
            List<string> desbloqueados = ManagerExperiencia.Instancia.ingredientesDesbloqueados;

            List<string> basesDisponibles = new List<string>();
            List<string> aderezosDisponibles = new List<string>();

            foreach (string ing in desbloqueados)
            {
                if (ing == "Carne" || ing == "Trompo" || ing == "Lechuga" || ing == "Porcion Lechuga")
                {
                    string nombreLimpio = (ing == "Trompo") ? "Carne" : (ing == "Porcion Lechuga") ? "Lechuga" : ing;
                    if (!basesDisponibles.Contains(nombreLimpio))
                        basesDisponibles.Add(nombreLimpio);
                }
                else
                {
                    if (!aderezosDisponibles.Contains(ing))
                        aderezosDisponibles.Add(ing);
                }
            }

            if (basesDisponibles.Count == 0) return;

            int cantidadBasesAElegir = (basesDisponibles.Count > 1 && Random.value > 0.4f) ? 2 : 1;

            List<string> basesMezcladas = new List<string>(basesDisponibles);
            for (int i = 0; i < basesMezcladas.Count; i++)
            {
                string temp = basesMezcladas[i];
                int randomIndex = Random.Range(i, basesMezcladas.Count);
                basesMezcladas[i] = basesMezcladas[randomIndex];
                basesMezcladas[randomIndex] = temp;
            }

            List<string> ingredientesElegidos = new List<string>();
            for (int i = 0; i < cantidadBasesAElegir; i++)
            {
                ingredientesElegidos.Add(basesMezcladas[i]);
            }

            
            if (aderezosDisponibles.Count > 0 && Random.value > 0.5f) 
            {
                int cantidadAderezos = Random.Range(1, Mathf.Min(2, aderezosDisponibles.Count + 1));

                List<string> aderezosMezclados = new List<string>(aderezosDisponibles);
                for (int i = 0; i < aderezosMezclados.Count; i++)
                {
                    string temp = aderezosMezclados[i];
                    int randomIndex = Random.Range(i, aderezosMezclados.Count);
                    aderezosMezclados[i] = aderezosMezclados[randomIndex];
                    aderezosMezclados[randomIndex] = temp;
                }

                for (int i = 0; i < cantidadAderezos; i++)
                {
                    ingredientesElegidos.Add(aderezosMezclados[i]);
                }
            }

            foreach (string item in ingredientesElegidos)
            {
                pedido.Enqueue(item);
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

        List<string> listaPedidoCliente = new List<string>(pedido);

        bool esCorrecto = true;

        if (ingredientesShawarmaEntregado == null || ingredientesShawarmaEntregado.Count != listaPedidoCliente.Count)
        {
            esCorrecto = false;
        }
        else
        {
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
                ManagerExperiencia.Instancia.RegistrarPedidoIncorrecto(10); 
            }
            Debug.Log("¡Pedido incorrecto! El cliente se va molesto.");
        }

        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarClienteAtendido();
        }

        if (miColaCliente != null)
        {
            miColaCliente.DestroyClientAtendido(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AtenderClienteExitoso()
    {
        if (atendidoOFrustrado) return;
        atendidoOFrustrado = true;

        List<string> listaPedidoCliente = new List<string>(pedido);
        List<string> ingredientesEntregados = ManagerExperiencia.ingredientesShawarmaEnMano;

        bool esCorrecto = true;

        if (ingredientesEntregados == null || ingredientesEntregados.Count != listaPedidoCliente.Count)
        {
            esCorrecto = false;
        }
        else
        {
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
                ManagerExperiencia.Instancia.RegistrarPedidoIncorrecto(10); 
            }
            Debug.Log("¡Pedido incorrecto o incompleto! Se penaliza al jugador.");
        }

        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarClienteAtendido();
        }

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