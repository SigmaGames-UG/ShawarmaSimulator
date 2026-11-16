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

            GameObject nuevaCarne = Instantiate(prefabCarne);

            inventario.AgregarHerramienta(nombreCarne, iconoCarne, nuevaCarne);

            
        }
    }
}