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
        // Al empezar el juego, nos aseguramos de que arranque vacío
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

            ActualizarVisuales(); // Revisamos si hay que cambiar el asset al cortar
        }
    }

    public void LimpiarTrompo()
    {
        tieneTrompo = false;
        usosActuales = 0;
        ActualizarVisuales();
    }

    // Esta es la magia que cambia los modelos
    private void ActualizarVisuales()
    {
        // 1. Apagamos todos por precaución para que no se superpongan
        if (visualVacia != null) visualVacia.SetActive(false);
        if (visualMedio != null) visualMedio.SetActive(false);
        if (visualLlena != null) visualLlena.SetActive(false);

        // 2. Encendemos el que corresponde según la cantidad de carne
        if (!tieneTrompo || usosActuales == 0)
        {
            if (visualVacia != null) visualVacia.SetActive(true);
        }
        else if (usosActuales >= 4) // De 4 a 7 usos (Trompo Lleno)
        {
            if (visualLlena != null) visualLlena.SetActive(true);
        }
        else if (usosActuales > 0 && usosActuales < 4) // De 1 a 3 usos (Trompo Medio)
        {
            if (visualMedio != null) visualMedio.SetActive(true);
        }
    }
}