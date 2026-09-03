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
        // Al empezar el juego, la bandeja arranca vacía
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

            // Creamos la porción y la mandamos a la mano
            GameObject nuevaPorcion = Instantiate(prefabPorcion);
            inventario.AgregarHerramienta(nombrePorcion, iconoPorcion, nuevaPorcion);

            ActualizarVisuales(); // Revisamos si hay que cambiar el asset
        }
    }

    // Esta función es la magia: decide qué asset mostrar
    private void ActualizarVisuales()
    {
        // 1. Apagamos todas por las dudas
        if (visualVacia != null) visualVacia.SetActive(false);
        if (visualMedio != null) visualMedio.SetActive(false);
        if (visualLlena != null) visualLlena.SetActive(false);

        // 2. Prendemos la que corresponde
        if (porcionesActuales == 0)
        {
            if (visualVacia != null) visualVacia.SetActive(true);
        }
        else if (porcionesActuales > 0 && porcionesActuales <= 4)
        {
            if (visualMedio != null) visualMedio.SetActive(true);
        }
        else // De 5 a 8 porciones
        {
            if (visualLlena != null) visualLlena.SetActive(true);
        }
    }
}