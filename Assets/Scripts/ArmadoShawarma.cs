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

        Quaternion rotacionDeseada = Quaternion.identity;

        switch (nombreIngredienteEnMano)
        {
            case "Carne":
                prefabAUsar = prefabCarnePlana;
                rotacionDeseada = Quaternion.Euler(-90f, 180f, 0f); 
                break;

            case "Lechuga":
            case "Porcion Lechuga":
                prefabAUsar = prefabLechugaPlana;
                nombreDefinitivo = "Lechuga";
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); 
                break;

            case "Ketchup":
                prefabAUsar = prefabKetchupPlano;
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); 
                break;

            case "Mayonesa":
                prefabAUsar = prefabMayoPlano;
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); 
                break;

            case "Tarator":
                prefabAUsar = prefabtaratorplano;
                rotacionDeseada = Quaternion.Euler(0f, 0f, 0f); 
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

        GameObject nuevoIngrediente = Instantiate(prefabAUsar, posicionAlta, rotacionDeseada);

        pilaIngredientes.Push(nuevoIngrediente);
        nombresIngredientes.Push(nombreDefinitivo);
    }


    public void TirarShawarma()
    {
        while (!pilaIngredientes.IsEmpty())
        {
            Destroy(pilaIngredientes.Pop());
            nombresIngredientes.Pop();
        }
    }

    public void CerrarShawarma(ToolManager inventario)
    {
        GameObject nuevoShawarma = Instantiate(prefabShawarmaTerminado);

        DatosShawarma datos = nuevoShawarma.GetComponentInChildren<DatosShawarma>();

        
        int cantidadTotal = pilaIngredientes.Count;
        string[] arrayTemporal = new string[cantidadTotal];
        List<string> listaParaExperiencia = new List<string>();

        int indice = 0;
        while (!pilaIngredientes.IsEmpty())
        {
            Destroy(pilaIngredientes.Pop());

            string nombreIng = nombresIngredientes.Pop();
            arrayTemporal[indice] = nombreIng;

            listaParaExperiencia.Add(nombreIng);

            indice++;
        }

        if (datos != null)
        {
            datos.ingredientesContenidos = arrayTemporal;
        }

        ManagerExperiencia.ingredientesShawarmaEnMano = listaParaExperiencia;

        inventario.AgregarHerramienta(nombreShawarma, iconoShawarma, nuevoShawarma);
    }
    public int CantidadIngredientes()
    {
        return pilaIngredientes.Count;
    }


}