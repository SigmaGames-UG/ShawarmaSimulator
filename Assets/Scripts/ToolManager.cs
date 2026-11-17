using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public TextMeshProUGUI textoEnPantalla;
    public Transform posicionMano;

    private ISimpleList<string> tools;
    private ISimpleList<Sprite> toolSprites;
    private ISimpleList<GameObject> toolModels;

    private Quaternion rotacionOriginal;
    private Vector3 escalaOriginal;

    void Start()
    {
        if (textoEnPantalla == null /*|| imagenEnPantalla == null*/)
            Debug.LogError("ToolManager: UI references are not assigned.", this);
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

    public void UsarHerramientaActual()
    {
        if (tools.Count == 0)
            return;

        GameObject objetoActual = toolModels.Get(0);

        AderezoUsable aderezo =
            objetoActual.GetComponentInChildren<AderezoUsable>();

        if (aderezo != null)
        {
            bool pudoUsarse = aderezo.Usar();

            if (!pudoUsarse)
                return;

            
            if (aderezo.EstaVacio())
            {
                ConsumirHerramientaActual();
            }
        }

        else
        {
            ConsumirHerramientaActual();
        }
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

        objetoFisico.transform.SetParent(posicionMano, true);

        ItemRecogible item = objetoFisico.GetComponent<ItemRecogible>();
        if (item != null)
        {
            objetoFisico.transform.localPosition = item.posicionEnMano;
            objetoFisico.transform.localRotation = Quaternion.Euler(item.rotacionEnMano);
        }
        else
        {
            objetoFisico.transform.localPosition = Vector3.zero;
            objetoFisico.transform.localRotation = Quaternion.identity;
        }


        Collider[] colliders = objetoFisico.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        Rigidbody[] rbs = objetoFisico.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        {
            r.isKinematic = true;
            r.useGravity = false;
        }

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

        objetoEnMano.transform.SetParent(null, true);

        objetoEnMano.transform.rotation = rotacionOriginal;
        objetoEnMano.transform.localScale = escalaOriginal;

        Collider[] colliders = objetoEnMano.GetComponentsInChildren<Collider>();
        float alturaParaSubir = 0.2f; 
        foreach (Collider c in colliders)
        {
            c.enabled = true;
            alturaParaSubir = c.bounds.extents.y;
        }

        Rigidbody[] rbs = objetoEnMano.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        {
            r.isKinematic = false;
            r.useGravity = true;
        }

        objetoEnMano.transform.position = posicionExactaMesa + new Vector3(0, alturaParaSubir + 0.1f, 0);

        tools.RemoveAt(0);
        toolSprites.RemoveAt(0);
        toolModels.RemoveAt(0);

        ActualizarPantalla();
    }

    void Update()
    {
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
    
    public GameObject ObtenerObjetoFisicoActual()
    {
        if (toolModels.Count > 0) return toolModels.Get(0);
        return null;
    }
    void ActualizarPantalla()
    {
        if (tools.Count == 0)
        {
            textoEnPantalla.text = "Manos vacías";
            return;
        }

        textoEnPantalla.text = "En la mano: " + tools.Get(0);
    }
}