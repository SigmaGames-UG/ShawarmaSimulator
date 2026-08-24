using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteraccionJugador : MonoBehaviour
{
    public Transform camara;
    public float distanciaInteraccion = 3f; 
    public ToolManager inventario;
    public TextMeshProUGUI textoAviso; 

    void Update()
    {
        RaycastHit hit;

        
        if (Physics.Raycast(camara.position, camara.forward, out hit, distanciaInteraccion))
        {
            
            ItemRecogible item = hit.collider.GetComponent<ItemRecogible>();

            if (item != null)
            {
                textoAviso.text = "Presiona E para agarrar " + item.nombreHerramienta;

                
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    
                    inventario.AgregarHerramienta(item.nombreHerramienta, item.iconoHerramienta);
                    Destroy(item.gameObject);
                }
            }
            else
            {
                textoAviso.text = ""; 
            }
        }
        else
        {
            textoAviso.text = ""; 
        }
    }
}