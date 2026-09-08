using UnityEngine;

public class ArmadoShawarma : MonoBehaviour
{
    // Nuestra estructura estática importada de la carpeta "Data Structures"
    private ISimpleStack<GameObject> pilaIngredientes = new SimpleArrayStack<GameObject>(10);

    public Transform puntoDeApoyo;
    public float alturaPorIngrediente = 0.05f;

    [Header("Futuros Assets (Ingredientes Cortados/Planos)")]
    public GameObject prefabCarnePlana;
    public GameObject prefabLechugaPlana;
    public GameObject prefabKetchupPlano;
    // Podés agregar más variables acá a medida que crees nuevos ingredientes

    public bool PuedeAgregar()
    {
        return pilaIngredientes.Count < 10;
    }

    public void AgregarIngrediente(string nombreIngredienteEnMano)
    {
        if (!PuedeAgregar()) return;

        GameObject prefabAUsar = null;

        // El switch revisa qué tenés en la mano y elige el asset plano correspondiente
        switch (nombreIngredienteEnMano)
        {
            case "Carne":
                prefabAUsar = prefabCarnePlana;
                break;
            case "Lechuga":
                prefabAUsar = prefabLechugaPlana;
                break;
            case "Ketchup":
                prefabAUsar = prefabKetchupPlano;
                break;
            default:
                Debug.LogWarning("Aún no configuraste un modelo plano para: " + nombreIngredienteEnMano);
                return;
        }

        // Si el asset de la ranura está vacío, no hace nada para evitar errores
        if (prefabAUsar == null) return;

        Vector3 posicionAlta = puntoDeApoyo.position + new Vector3(0, pilaIngredientes.Count * alturaPorIngrediente, 0);

        GameObject nuevoIngrediente = Instantiate(prefabAUsar, posicionAlta, Quaternion.identity);
        nuevoIngrediente.transform.SetParent(puntoDeApoyo);

        // ¡Lo metemos en la Pila!
        pilaIngredientes.Push(nuevoIngrediente);
    }

    // El jugador se equivocó: ¡A la basura!
    public void TirarShawarma()
    {
        // Mientras la pila NO esté vacía, sacamos el de más arriba y lo destruimos
        while (!pilaIngredientes.IsEmpty())
        {
            GameObject ingredienteArruinado = pilaIngredientes.Pop();
            Destroy(ingredienteArruinado);
        }
    }

    public int CantidadIngredientes()
    {
        return pilaIngredientes.Count;
    }
}