using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    
    public GameObject panelAjustes;

    public void BotonInicio()
    {
       
        SceneManager.LoadScene("SampleScene");
    }

    public void BotonAjustes()
    {
        panelAjustes.SetActive(true); 
    }

    public void CerrarAjustes()
    {
        panelAjustes.SetActive(false); 
    }

    public void BotonSalir()
    {
        
       
        Application.Quit();
    }
}
