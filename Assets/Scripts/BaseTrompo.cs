using UnityEngine;

public class BaseTrompo : MonoBehaviour
{
    public bool tieneTrompo = false;
    public int usosMaximos = 7;
    public int usosActuales = 0;

    [Header("Visuales y Prefabs")]
    // Este es el modelo 3D gigante del trompo que va a estar FIJO en la máquina (arranca apagado)
    public GameObject modeloTrompoVisual;

    // Este es el cubito de carne que te va a dar a la mano
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
        if (modeloTrompoVisual != null) modeloTrompoVisual.SetActive(true); // ¡Aparece el trompo!
    }

    public void SacarCarne(ToolManager inventario)
    {
        if (tieneTrompo && usosActuales > 0 && !inventario.TieneHerramientas())
        {
            usosActuales--;

            // Creamos la carne y la teletransportamos a tu mano
            GameObject nuevaCarne = Instantiate(prefabCarne);
            inventario.AgregarHerramienta(nombreCarne, iconoCarne, nuevaCarne);
        }
    }

    public void LimpiarTrompo()
    {
        tieneTrompo = false;
        usosActuales = 0;
        if (modeloTrompoVisual != null) modeloTrompoVisual.SetActive(false); // ¡Desaparece el trompo!
    }
}