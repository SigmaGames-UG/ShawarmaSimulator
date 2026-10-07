using UnityEngine;

public class ArmadoShawarma : MonoBehaviour
{
    private ISimpleStack<GameObject> pilaIngredientes =
        new SimpleArrayStack<GameObject>(10);

    private ISimpleStack<string> nombresIngredientes =
        new SimpleArrayStack<string>(10);

    public Transform puntoDeApoyo;
    public float alturaPorIngrediente = 0.05f;

    [Header("Ingredientes Cortados/Planos")]
    public GameObject prefabCarnePlana;
    public GameObject prefabLechugaPlana;
    public GameObject prefabKetchupPlano;
    public GameObject prefabMayoPlano;
    public GameObject prefabtaratorplano;

    [Header("Shawarma Terminado")]
    public GameObject prefabShawarmaTerminado;
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
        string nombreDefinitivo = nombreIngredienteEnMano;

        switch (nombreIngredienteEnMano)
        {
            case "Carne":
                prefabAUsar = prefabCarnePlana;
                break;

            case "Lechuga":
            case "Porcion Lechuga":
                prefabAUsar = prefabLechugaPlana;
                nombreDefinitivo = "Lechuga";
                break;

            case "Ketchup":
                prefabAUsar = prefabKetchupPlano;
                break;

            case "Mayonesa":
                prefabAUsar = prefabMayoPlano;
                break;

            case "Tarator":
                prefabAUsar = prefabtaratorplano;
                break;


            default:
                Debug.LogWarning(
                    "Aún no configuraste un modelo plano para: "
                    + nombreIngredienteEnMano
                );
                return;
        }



        if (prefabAUsar == null) return;

        Vector3 posicionAlta = puntoDeApoyo.position + new Vector3(0, pilaIngredientes.Count * alturaPorIngrediente, 0);

        GameObject nuevoIngrediente = Instantiate(prefabAUsar, posicionAlta, Quaternion.identity);
        nuevoIngrediente.transform.SetParent(puntoDeApoyo, true);

        // Guardamos el modelo visual por un lado, y el texto por el otro
        pilaIngredientes.Push(nuevoIngrediente);
        nombresIngredientes.Push(nombreDefinitivo);
    }



    public void TirarShawarma()
    {
        // ¡Volvemos a ponerle los paréntesis a IsEmpty()!
        while (!pilaIngredientes.IsEmpty())
        {
            Destroy(pilaIngredientes.Pop());
            nombresIngredientes.Pop();
        }
    }

    // Envolver y entregarlo a la mano
    public void CerrarShawarma(ToolManager inventario)
    {
        // 1. Instanciamos el shawarma envuelto
        GameObject nuevoShawarma = Instantiate(prefabShawarmaTerminado);

        // ¡AGREGAMOS IN CHILDREN ACÁ TAMBIÉN!
        DatosShawarma datos = nuevoShawarma.GetComponentInChildren<DatosShawarma>();

        // 2. Creamos un arreglo normal de C# del tamaño exacto de tu pila
        int cantidadTotal = pilaIngredientes.Count;
        string[] arrayTemporal = new string[cantidadTotal];

        // 3. Vaciamos las pilas paso a paso
        int indice = 0;
        while (!pilaIngredientes.IsEmpty())
        {
            // Destruimos el modelo 3D plano de la mesa
            Destroy(pilaIngredientes.Pop());

            // Guardamos el texto (ej: "Lechuga") en nuestro arreglo temporal
            arrayTemporal[indice] = nombresIngredientes.Pop();
            indice++;
        }

        // 4. Le pasamos el arreglo ya armado al script del shawarma final
        if (datos != null)
        {
            datos.ingredientesContenidos = arrayTemporal;
        }

        // 5. Se lo damos al jugador
        inventario.AgregarHerramienta(nombreShawarma, iconoShawarma, nuevoShawarma);
    }

    public int CantidadIngredientes()
    {
        return pilaIngredientes.Count;
    }


}