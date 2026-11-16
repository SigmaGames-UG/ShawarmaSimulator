using UnityEngine;

public class BandejaLechuga : MonoBehaviour
{
    public int porcionesTotales = 8;
    public int porcionesActuales = 0;

    [Header("Modelos Visuales (Hijos)")]
    public GameObject visualVacia;
    public GameObject visualMedio;
    public GameObject visualLlena;

    [Header("Datos de la Porción (Para la mano)")]
    public GameObject prefabPorcion;
    public string nombrePorcion = "Porcion Lechuga";
    public Sprite iconoPorcion;

    void Start()
    {
        ActualizarVisuales();
    }

    public void LlenarBandeja()
    {
        porcionesActuales = porcionesTotales;
        ActualizarVisuales();
    }

    public void SacarPorcion(ToolManager inventario)
    {
        if (porcionesActuales > 0 && !inventario.TieneHerramientas())
        {
            porcionesActuales--;

            GameObject nuevaPorcion = Instantiate(prefabPorcion);
            inventario.AgregarHerramienta(nombrePorcion, iconoPorcion, nuevaPorcion);

            ActualizarVisuales(); 
        }
    }


    private void ActualizarVisuales()
    {
        if (visualVacia != null) visualVacia.SetActive(false);
        if (visualMedio != null) visualMedio.SetActive(false);
        if (visualLlena != null) visualLlena.SetActive(false);

        if (porcionesActuales == 0)
        {
            if (visualVacia != null) visualVacia.SetActive(true);
        }
        else if (porcionesActuales > 0 && porcionesActuales <= 4)
        {
            if (visualMedio != null) visualMedio.SetActive(true);
        }
        else 
        {
            if (visualLlena != null) visualLlena.SetActive(true);
        }
    }
}