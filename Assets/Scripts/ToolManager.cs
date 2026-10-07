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

    public void UsarHerramientaActual()
    {
        if (tools.Count == 0)
            return;

        GameObject objetoActual = toolModels.Get(0);

        AderezoUsable aderezo =
            objetoActual.GetComponentInChildren<AderezoUsable>();

        // Si es un aderezo
        if (aderezo != null)
        {
            bool pudoUsarse = aderezo.Usar();

            if (!pudoUsarse)
                return;

            // Si gastamos el último uso,
            // destruimos la botella
            if (aderezo.EstaVacio())
            {
                ConsumirHerramientaActual();
            }
        }

        // Si NO es un aderezo (carne, lechuga, etc.)
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

        // Guardamos cómo era originalmente
        rotacionOriginal = objetoFisico.transform.rotation;
        escalaOriginal = objetoFisico.transform.localScale;

        // Lo colocamos en la mano
        // 1. Usamos TRUE para que el objeto no se vuelva gigante o enano al pegarse a la mano
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

        // Ya no necesitamos forzar la escala porque SetParent(..., true) la protege

        // 2. Apagamos TODOS los colliders de la pieza y sus hijos (esto ya lo hacías bien)
        Collider[] colliders = objetoFisico.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        // 3. ¡LA SOLUCIÓN AL BUG INCONTROLABLE! Apagamos TODOS los Rigidbodies
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

        // 1. Lo desvinculamos de la mano con "true" para que no herede tamaños raros
        objetoEnMano.transform.SetParent(null, true);

        objetoEnMano.transform.rotation = rotacionOriginal;
        objetoEnMano.transform.localScale = escalaOriginal;

        // 2. Volvemos a prender las colisiones y calculamos la altura
        Collider[] colliders = objetoEnMano.GetComponentsInChildren<Collider>();
        float alturaParaSubir = 0.2f; // Lo declaramos UNA sola vez

        foreach (Collider c in colliders)
        {
            c.enabled = true;
            // Usamos el tamaño del primer collider que encuentre para calcular la altura
            alturaParaSubir = c.bounds.extents.y;
        }

        // 3. ¡VOLVEMOS A PRENDER LA FÍSICA Y LA GRAVEDAD PARA TODO EL OBJETO!
        Rigidbody[] rbs = objetoEnMano.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        {
            r.isKinematic = false;
            r.useGravity = true;
        }

        // 4. Lo posicionamos usando la altura que ya calculamos arriba
        objetoEnMano.transform.position = posicionExactaMesa + new Vector3(0, alturaParaSubir + 0.1f, 0);

        // 5. Vaciamos la mano
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
            imagenEnPantalla.enabled = false;
            return;
        }

        imagenEnPantalla.enabled = true;
        textoEnPantalla.text = "En la mano: " + tools.Get(0);
        imagenEnPantalla.sprite = toolSprites.Get(0);
    }
}