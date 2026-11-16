using UnityEngine;

public class BaseTrompo : MonoBehaviour
{
    public bool tieneTrompo = false;
    public int usosMaximos = 7;
    public int usosActuales = 0;

    [Header("Modelos Visuales (Hijos)")]
    public GameObject visualVacia;
    public GameObject visualMedio;
    public GameObject visualLlena;

    [Header("Datos de la Porción (Para la mano)")]
    public GameObject prefabCarne;
    public string nombreCarne = "Carne";
    public Sprite iconoCarne;

    void Start()
    {
        LimpiarTrompo();
    }

    public void ColocarTrompo()
    {
        tieneTrompo = true;
        usosActuales = usosMaximos;
        ActualizarVisuales();
    }

    public void SacarCarne(ToolManager inventario)
    {
        if (tieneTrompo && usosActuales > 0 && !inventario.TieneHerramientas())
        {
            usosActuales--;

            GameObject nuevaCarne = Instantiate(prefabCarne);
            inventario.AgregarHerramienta(nombreCarne, iconoCarne, nuevaCarne);

            ActualizarVisuales(); 
        }
    }

    public void LimpiarTrompo()
    {
        tieneTrompo = false;
        usosActuales = 0;
        ActualizarVisuales();
    }

    private void ActualizarVisuales()
    {
        if (visualVacia != null) visualVacia.SetActive(false);
        if (visualMedio != null) visualMedio.SetActive(false);
        if (visualLlena != null) visualLlena.SetActive(false);

        if (!tieneTrompo || usosActuales == 0)
        {
            if (visualVacia != null) visualVacia.SetActive(true);
        }
        else if (usosActuales >= 4) 
        {
            if (visualLlena != null) visualLlena.SetActive(true);
        }
        else if (usosActuales > 0 && usosActuales < 4) 
        {
            if (visualMedio != null) visualMedio.SetActive(true);
        }
    }
}