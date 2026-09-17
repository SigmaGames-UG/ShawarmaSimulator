using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public TextMeshProUGUI textoEnPantalla;
    public Image imagenEnPantalla;
    public Transform posicionMano;

    private ISimpleList<string> tools;
    private ISimpleList<Sprite> toolSprites;
    private ISimpleList<GameObject> toolModels;

    private Quaternion rotacionOriginal;
    private Vector3 escalaOriginal;

    void Start()
    {
        tools = new SimpleArrayList<string>(1);
        toolSprites = new SimpleArrayList<Sprite>(1);
        toolModels = new SimpleArrayList<GameObject>(1);
        ActualizarPantalla();
    }
    
    public string ObtenerNombreActual()
    {
        if (tools.Count > 0) return tools.Get(0);
        return "";
    }

    
    public void ConsumirHerramientaActual()
    {
        if (tools.Count == 0) return;

        Destroy(toolModels.Get(0));
        tools.RemoveAt(0);
        toolSprites.RemoveAt(0);
        toolModels.RemoveAt(0);

        ActualizarPantalla();
    }

    public void AgregarHerramienta(string nombre, Sprite icono, GameObject objetoFisico)
    {
        if (tools.Count >= 1) return;

        tools.Add(nombre);
        toolSprites.Add(icono);
        toolModels.Add(objetoFisico);

        rotacionOriginal = objetoFisico.transform.rotation;
        escalaOriginal = objetoFisico.transform.localScale;

        objetoFisico.transform.SetParent(posicionMano);
        objetoFisico.transform.localPosition = Vector3.zero;
        objetoFisico.transform.localRotation = Quaternion.identity;
        objetoFisico.transform.localScale = new Vector3(1f, 1f, 1f);

        Collider col = objetoFisico.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        ActualizarPantalla();
    }

    public bool TieneHerramientas()
    {
        return tools.Count > 0;
    }

    public void SoltarHerramientaActual(Vector3 posicionExactaMesa)
    {
        if (tools.Count == 0) return;

        GameObject objetoEnMano = toolModels.Get(0);

     
        objetoEnMano.transform.SetParent(null);

        
        objetoEnMano.transform.rotation = rotacionOriginal;
        objetoEnMano.transform.localScale = escalaOriginal;

        
        Collider col = objetoEnMano.GetComponent<Collider>();
        if (col != null) col.enabled = true;

       
        
        float alturaParaSubir = 60f;
        if (col != null)
        {
            alturaParaSubir = col.bounds.extents.y;
        }

        
        objetoEnMano.transform.position = posicionExactaMesa + new Vector3(0, alturaParaSubir, 0);

        
        tools.RemoveAt(0);
        toolSprites.RemoveAt(0);
        toolModels.RemoveAt(0);

        ActualizarPantalla();
    }

    void Update()
    {
        // El sistema de abrir la caja con la T que ya andaba bárbaro
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame && tools.Count > 0)
        {
            GameObject objetoEnMano = toolModels.Get(0);
            CajaDelivery infoCaja = objetoEnMano.GetComponent<CajaDelivery>();

            if (infoCaja != null)
            {
                GameObject nuevaSalsa = Instantiate(infoCaja.prefabContenido);

                tools.RemoveAt(0);
                toolSprites.RemoveAt(0);
                toolModels.RemoveAt(0);
                Destroy(objetoEnMano);

                AgregarHerramienta(infoCaja.nombreContenido, infoCaja.iconoContenido, nuevaSalsa);
            }
        }
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
        textoEnPantalla.text = "En la mano: " + tools.Get(0);
        imagenEnPantalla.sprite = toolSprites.Get(0);
    }
}