using UnityEngine;

public class BaseTrompo : MonoBehaviour
{
    public bool tieneTrompo = false;
    public int usosMaximos = 7;
    public int usosActuales = 0;

    [Header("Visuales y Prefabs")]
   
    public GameObject modeloTrompoVisual;

    
    public GameObject prefabCarne;
    public string nombreCarne = "Carne";
    public Sprite iconoCarne;

    void Start()
    {
        // Al empezar el juego, la máquina está vacía
        if (modeloTrompoVisual != null) modeloTrompoVisual.SetActive(false);
    }

    public void ColocarTrompo()
    {
        tieneTrompo = true;
        usosActuales = usosMaximos;
        if (modeloTrompoVisual != null) modeloTrompoVisual.SetActive(true); 
    }

    public void SacarCarne(ToolManager inventario)
    {
        if (tieneTrompo && usosActuales > 0 && !inventario.TieneHerramientas())
        {
            usosActuales--;

           
            GameObject nuevaCarne = Instantiate(prefabCarne);
            inventario.AgregarHerramienta(nombreCarne, iconoCarne, nuevaCarne);
        }
    }

    public void LimpiarTrompo()
    {
        tieneTrompo = false;
        usosActuales = 0;
        if (modeloTrompoVisual != null) modeloTrompoVisual.SetActive(false); 
    }
}