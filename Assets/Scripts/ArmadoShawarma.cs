using UnityEngine;

public class ArmadoShawarma : MonoBehaviour
{
    private ISimpleStack<GameObject> pilaIngredientes = new SimpleArrayStack<GameObject>(10);

    public Transform puntoDeApoyo;
    public float alturaPorIngrediente = 0.05f;

    [Header("Futuros Assets (Ingredientes Cortados/Planos)")]
    public GameObject prefabCarnePlana;
    public GameObject prefabLechugaPlana;
    public GameObject prefabKetchupPlano;

    [Header("Shawarma Terminado")]
    public GameObject prefabShawarmaTerminado; // El asset del shawarma ya envuelto
    public string nombreShawarma = "Shawarma";
    public Sprite iconoShawarma;

    public bool PuedeAgregar()
    {
        return pilaIngredientes.Count < 10;
    }

    public void AgregarIngrediente(string nombreIngredienteEnMano)
    {
        if (!PuedeAgregar()) return;

        GameObject prefabAUsar = null;

        switch (nombreIngredienteEnMano)
        {
            case "Carne": prefabAUsar = prefabCarnePlana; break;
            case "Lechuga": prefabAUsar = prefabLechugaPlana; break;
            case "Ketchup": prefabAUsar = prefabKetchupPlano; break;
            default:
                Debug.LogWarning("Aún no configuraste un modelo plano para: " + nombreIngredienteEnMano);
                return;
        }

        if (prefabAUsar == null) return;

        Vector3 posicionAlta = puntoDeApoyo.position + new Vector3(0, pilaIngredientes.Count * alturaPorIngrediente, 0);
        GameObject nuevoIngrediente = Instantiate(prefabAUsar, posicionAlta, Quaternion.identity);
        nuevoIngrediente.transform.SetParent(puntoDeApoyo);

        pilaIngredientes.Push(nuevoIngrediente);
    }

    public void TirarShawarma()
    {
        while (!pilaIngredientes.IsEmpty())
        {
            Destroy(pilaIngredientes.Pop());
        }
    }

    // ¡NUEVA FUNCIÓN! Envolver y entregarlo a la mano
    public void CerrarShawarma(ToolManager inventario)
    {
        // Vaciamos visual y lógicamente el plato
        while (!pilaIngredientes.IsEmpty())
        {
            Destroy(pilaIngredientes.Pop());
        }

        // Instanciamos el Shawarma terminado y te lo damos
        GameObject nuevoShawarma = Instantiate(prefabShawarmaTerminado);
        inventario.AgregarHerramienta(nombreShawarma, iconoShawarma, nuevoShawarma);
    }

    public int CantidadIngredientes()
    {
        return pilaIngredientes.Count;
    }
}