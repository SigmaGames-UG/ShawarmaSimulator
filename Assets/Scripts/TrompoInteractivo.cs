using UnityEngine;

public class TrompoInteractivo : MonoBehaviour
{
    public int usosMaximos = 7;
    public int usosActuales;

    [Header("Datos de la Carne")]
    public string nombreCarne = "Carne";
    public Sprite iconoCarne;
    public GameObject prefabCarne; 

    void Start()
    {
        usosActuales = usosMaximos;
    }

    public void SacarCarne(ToolManager inventario)
    {
       
        if (usosActuales > 0 && !inventario.TieneHerramientas())
        {
            usosActuales--;

            // 1. Instanciamos el objeto físico de la carne
            GameObject nuevaCarne = Instantiate(prefabCarne);

            // 2. Lo teletransportamos directo a la mano usando tu sistema
            inventario.AgregarHerramienta(nombreCarne, iconoCarne, nuevaCarne);

            // Opcional para el futuro: achicar el trompo
            // visualmente para que parezca que se va consumiendo.
        }
    }
}