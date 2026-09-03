using UnityEngine;

public class TrompoInteractivo : MonoBehaviour
{
    public int usosMaximos = 7;
    public int usosActuales;

    [Header("Datos de la Carne")]
    public string nombreCarne = "Carne";
    public Sprite iconoCarne;
    public GameObject prefabCarne; // El prefab 3D de la carne (¡Asegúrate de que tenga ItemRecogible y BoxCollider!)

    void Start()
    {
        usosActuales = usosMaximos;
    }

    public void SacarCarne(ToolManager inventario)
    {
        // Solo saca carne si quedan usos y tienes las manos libres
        if (usosActuales > 0 && !inventario.TieneHerramientas())
        {
            usosActuales--;

            // 1. Instanciamos el objeto físico de la carne
            GameObject nuevaCarne = Instantiate(prefabCarne);

            // 2. Lo teletransportamos directo a tu mano usando tu sistema
            inventario.AgregarHerramienta(nombreCarne, iconoCarne, nuevaCarne);

            // Opcional para el futuro: Aquí podrías achicar la escala del trompo 
            // visualmente para que parezca que se va consumiendo.
        }
    }
}