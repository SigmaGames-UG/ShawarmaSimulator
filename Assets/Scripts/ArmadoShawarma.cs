using System.Collections.Generic;
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

        // Declaramos la rotación específica para cada ingrediente
        Quaternion rotacionDeseada = Quaternion.identity;

        switch (nombreIngredienteEnMano)
        {
            case "Carne":
                prefabAUsar = prefabCarnePlana;
                rotacionDeseada = Quaternion.Euler(-90f, 180f, 0f); // Ajusta aquí los grados para la carne si lo necesitas
                break;

            case "Lechuga":
            case "Porcion Lechuga":
                prefabAUsar = prefabLechugaPlana;
                nombreDefinitivo = "Lechuga";
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); // Ajusta aquí los grados para la lechuga
                break;

            case "Ketchup":
                prefabAUsar = prefabKetchupPlano;
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); // Ajusta aquí los grados para el ketchup
                break;

            case "Mayonesa":
                prefabAUsar = prefabMayoPlano;
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); // Ajusta aquí los grados para la mayonesa
                break;

            case "Tarator":
                prefabAUsar = prefabtaratorplano;
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); // Ajusta aquí los grados para el tarator
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

        // Instanciamos el ingrediente aplicando su rotación individual correspondiente
        GameObject nuevoIngrediente = Instantiate(prefabAUsar, posicionAlta, rotacionDeseada);

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

        DatosShawarma datos = nuevoShawarma.GetComponentInChildren<DatosShawarma>();

        // ==========================================
        // PASO 2: Creamos el arreglo y la lista temporal
        // ==========================================
        int cantidadTotal = pilaIngredientes.Count;
        string[] arrayTemporal = new string[cantidadTotal];
        List<string> listaParaExperiencia = new List<string>();

        // 3. Vaciamos las pilas paso a paso
        int indice = 0;
        while (!pilaIngredientes.IsEmpty())
        {
            // Destruimos el modelo 3D plano de la mesa
            Destroy(pilaIngredientes.Pop());

            // Guardamos el texto (ej: "Lechuga") en nuestro arreglo temporal
            string nombreIng = nombresIngredientes.Pop();
            arrayTemporal[indice] = nombreIng;

            // Añadimos también a la lista para validar la entrega contra el cliente
            listaParaExperiencia.Add(nombreIng);

            indice++;
        }

        // 4. Le pasamos el arreglo ya armado al script del shawarma final
        if (datos != null)
        {
            datos.ingredientesContenidos = arrayTemporal;
        }

        // Guardamos los ingredientes en la mano para que el Cliente los evalúe rigurosamente al entregar
        ManagerExperiencia.ingredientesShawarmaEnMano = listaParaExperiencia;

        // 5. Se lo damos al jugador
        inventario.AgregarHerramienta(nombreShawarma, iconoShawarma, nuevoShawarma);
    }
    public int CantidadIngredientes()
    {
        return pilaIngredientes.Count;
    }


}