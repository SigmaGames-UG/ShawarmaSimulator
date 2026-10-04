using UnityEngine;

public class Cliente : MonoBehaviour
{
    public enum Estado { EsperandoAtencion, EsperandoComida, Satisfecho }
    public Estado estadoActual = Estado.EsperandoAtencion;
    private bool moving = false;
    private int speed = 140;
    private Vector3 targetPosition;

    public ISimpleSet<string> pedido = new SimpleArraySet<string>(5);
    public ClienteQueue clienteQueue;

    void Start()
    {
        GenerarPedido();
    }

    void GenerarPedido()
    {
        pedido.Add("Carne");

        string[] ingredientesPosibles = ManagerExperiencia.Instancia.ingredientesDesbloqueados.ToArray();

        int cantidadExtras = Random.Range(0, 4);
        int totalDeseado = 1 + cantidadExtras;

        if (totalDeseado > ingredientesPosibles.Length) totalDeseado = ingredientesPosibles.Length;

        while (pedido.Count < totalDeseado)
        {
            string ingredienteAzar = ingredientesPosibles[Random.Range(0, ingredientesPosibles.Length)];
            pedido.Add(ingredienteAzar);
        }
    }

    public string ObtenerTextoPedido()
    {
        return string.Join(" + ", pedido.ToArray());
    }

    public void AceptarPedido()
    {
        estadoActual = Estado.EsperandoComida;
    }

    public void RecibirComida(ToolManager inventario)
    {
        GameObject objEnMano = inventario.ObtenerObjetoFisicoActual();

        if (objEnMano != null)
        {
            // Buscamos el componente de datos
            DatosShawarma datos = objEnMano.GetComponentInChildren<DatosShawarma>();

            // ¡TRUCO DE SEGURIDAD! Si por alguna razón sigue sin tenerlo, se lo agregamos en el aire
            if (datos == null)
            {
                Debug.LogWarning("El objeto no tenía el script DatosShawarma. Se lo agregamos automáticamente.");
                datos = objEnMano.AddComponent<DatosShawarma>();
                // Le inventamos un contenido básico para que no explote
                datos.ingredientesContenidos = new string[] { "Carne" };
            }

            // Evaluamos la receta
            if (EvaluarShawarma(datos.ingredientesContenidos))
            {
                int xpGanada = pedido.Count * 15;
                ManagerExperiencia.Instancia.ModificarXP(xpGanada);
                Debug.Log("¡Cliente Feliz! Ganaste " + xpGanada + " XP");
            }
            else
            {
                ManagerExperiencia.Instancia.ModificarXP(-20);
                Debug.Log("¡Receta equivocada! Perdiste 20 XP");
            }
        }
        else
        {
            Debug.Log("No tenías nada en la mano.");
        }

        inventario.ConsumirHerramientaActual();
        estadoActual = Estado.Satisfecho;
        Destroy(gameObject);
    }

    // Algoritmo para verificar que los arreglos coincidan exactamente
    private bool EvaluarShawarma(string[] entregados)
    {
        string[] pedidosArray = pedido.ToArray();

        // --- IMPRIMIMOS EN CONSOLA PARA DEPURAR ---
        Debug.Log("--- EVALUANDO PEDIDO ---");
        Debug.Log("Cliente pidió: " + string.Join(" | ", pedidosArray));
        Debug.Log("Shawarma entregado: " + (entregados != null ? string.Join(" | ", entregados) : "VACÍO O NULL"));

        if (entregados == null) return false;

        // Si la cantidad de ingredientes es distinta, ya está mal de entrada
        if (pedidosArray.Length != entregados.Length)
        {
            Debug.Log("¡Error! La cantidad de ingredientes no coincide.");
            return false;
        }

        // Verificamos que cada ingrediente pedido esté en el shawarma entregado
        foreach (string p in pedidosArray)
        {
            bool encontrado = false;
            foreach (string e in entregados)
            {
                // Comparamos limpiando espacios y mayúsculas para evitar errores tontos
                if (p.Trim().ToLower() == e.Trim().ToLower())
                {
                    encontrado = true;
                    break;
                }
            }
            if (!encontrado)
            {
                Debug.Log("¡Error! Falta el ingrediente: " + p);
                return false;
            }
        }

        Debug.Log("¡Receta perfecta! Sumando XP...");
        return true; // Si llegó hasta acá, el shawarma es perfecto
    }

    private void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        if (transform.position == targetPosition) moving = false;
    }

    public void MoveTo(Vector3 Position)
    {
        targetPosition = Position;
        moving = true;
    }
}