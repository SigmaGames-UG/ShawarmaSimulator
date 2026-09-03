using UnityEngine;
using UnityEngine.SceneManagement; // ¡Línea vital para cambiar escenas!

public class MenuManager : MonoBehaviour
{
    // Acá arrastraremos nuestro panel de ajustes desde el Inspector
    public GameObject panelAjustes;

    public void BotonInicio()
    {
        // Pongo "SampleScene" porque vi en tus capturas que así se llama tu escena de juego.
        // Si le cambiaste el nombre, escribí el nombre exacto acá.
        SceneManager.LoadScene("SampleScene");
    }

    public void BotonAjustes()
    {
        panelAjustes.SetActive(true); // Prende el panel
    }

    public void CerrarAjustes()
    {
        panelAjustes.SetActive(false); // Apaga el panel
    }

    public void BotonSalir()
    {
        // En el editor de Unity, Quit() no hace nada visualmente, por eso ponemos un mensaje.
        // Pero cuando exportes el juego a un .exe, esto lo va a cerrar de verdad.
        Debug.Log("¡Cerrando el juego!");
        Application.Quit();
    }
}
