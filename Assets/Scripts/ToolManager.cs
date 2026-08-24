using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public TextMeshProUGUI textoEnPantalla;
    public Image imagenEnPantalla;

    private ISimpleList<string> tools;
    private ISimpleList<Sprite> toolSprites;
    private int indiceActual = 0;

    void Start()
    {
        
        tools = new SimpleArrayList<string>(3);
        toolSprites = new SimpleArrayList<Sprite>(3);
        ActualizarPantalla();
    }

    void Update()
    {
        
        if (tools.Count == 0) return;

        
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            indiceActual++;
            if (indiceActual >= tools.Count) indiceActual = 0;
            ActualizarPantalla();
        }
    }

    
    public void AgregarHerramienta(string nombre, Sprite icono)
    {
        tools.Add(nombre);
        toolSprites.Add(icono);

        if (tools.Count == 1)
        {
            indiceActual = 0;
        }

        ActualizarPantalla();
    }

    void ActualizarPantalla()
    {
       
        if (tools.Count == 0)
        {
            textoEnPantalla.text = "Manos vacías";
            imagenEnPantalla.enabled = false;
            return;
        }

      
        imagenEnPantalla.enabled = true;
        textoEnPantalla.text = "En la mano: " + tools.Get(indiceActual);
        imagenEnPantalla.sprite = toolSprites.Get(indiceActual);
    }
}