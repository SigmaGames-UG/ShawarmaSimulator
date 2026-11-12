using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TiendaManager : MonoBehaviour
{
    public GameObject uiCelular;
    public GameObject panelTienda;
    public GameObject panelCarrito;
    public Transform puntoEntrega;
    public GameObject prefabCaja;
   

    [Header("Ordenamiento de botones")]
    public Transform panelBotonesTienda;

    private IngredienteBoton[] botonesIngredientes;

    public MonoBehaviour scriptCamara;

    [Header("Bases de Datos de Salsas")]
    public GameObject prefabKetchup;
    public GameObject prefabtrompo;
    public GameObject prefabMayo;
    public GameObject prefabTarator;
    public GameObject prefablechuga;
    public Sprite iconKetchup;
    public Sprite iconMayo;
    public Sprite iconTarator;
    public Sprite icontrompo;
    public Sprite iconLechuga;

    private List<GameObject> carritoPrefabs = new List<GameObject>();
    private List<string> carritoNombres = new List<string>();
    private List<Sprite> carritoIconos = new List<Sprite>();

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            uiCelular.SetActive(!uiCelular.activeSelf);

            if (uiCelular.activeSelf)
            {
                AbrirTienda();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (scriptCamara != null) scriptCamara.enabled = false;
            }
            else
            {
                CerrarCelular();
            }
        }
    }

    private void OrdenarBotones()
    {
        if (panelBotonesTienda == null) return;

        int cantidad = panelBotonesTienda.childCount;
        botonesIngredientes = new IngredienteBoton[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            botonesIngredientes[i] =
                panelBotonesTienda.GetChild(i)
                .GetComponent<IngredienteBoton>();
        }

        CollectionSort.SortArray(botonesIngredientes);

        for (int i = 0; i < botonesIngredientes.Length; i++)
        {
            if (botonesIngredientes[i] != null)
            {
                botonesIngredientes[i].transform.SetSiblingIndex(i);
            }
        }
    }

    public void AbrirTienda()
    {
        if (panelTienda != null) panelTienda.SetActive(true);
        if (panelCarrito != null) panelCarrito.SetActive(false);
        OrdenarBotones();
        ActualizarEstadoBotonesTienda(); // <--- Llamamos a la verificación al abrir
    }
    private void ActualizarEstadoBotonesTienda()
    {
        if (botonesIngredientes == null) return;

        foreach (var botonIngrediente in botonesIngredientes)
        {
            if (botonIngrediente != null)
            {
                // Buscamos el texto dentro del botón (TextMeshProUGUI)
                TextMeshProUGUI textoBoton = botonIngrediente.GetComponentInChildren<TextMeshProUGUI>();

                if (textoBoton != null)
                {
                    string nombreIngrediente = textoBoton.text; // Usa el texto que muestra el botón
                    bool desbloqueado = EstaDesbloqueado(nombreIngrediente);

                    Button btn = botonIngrediente.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.interactable = desbloqueado;
                    }
                }
            }
        }
    }

    public void AbrirCarrito()
    {
        if (panelTienda != null) panelTienda.SetActive(false);
        if (panelCarrito != null) panelCarrito.SetActive(true);
    }

    public void CerrarCelular()
    {
        if (uiCelular != null) uiCelular.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (scriptCamara != null) scriptCamara.enabled = true;
    }

    // --- MÉTODOS DE COMPRA DIRECTOS ---
    // --- MÉTODOS DE COMPRA SEGUROS Y BLOQUEADOS ---
    public void AñadirKetchup()
    {
        if (EstaDesbloqueado("Ketchup"))
        {
            Agregar(prefabKetchup, "Ketchup", iconKetchup);
        }
        else
        {
            Debug.Log("¡Ketchup bloqueado! Debes desbloquearlo con experiencia primero.");
        }
    }

    public void AñadirMayo()
    {
        if (EstaDesbloqueado("Mayonesa"))
        {
            Agregar(prefabMayo, "Mayonesa", iconMayo);
        }
        else
        {
            Debug.Log("¡Mayonesa bloqueada! Debes desbloquearla con experiencia primero.");
        }
    }

    public void AñadirTarator()
    {
        if (EstaDesbloqueado("Tarator"))
        {
            Agregar(prefabTarator, "Tarator", iconTarator);
        }
        else
        {
            Debug.Log("¡Tarator bloqueado! Debes desbloquearlo con experiencia primero.");
        }
    }

    public void AñadirTrompo() 
    { 
        // Permitimos comprar si está desbloqueado como "Trompo" o "Carne", 
        // o si queremos que la carne y la lechuga estén siempre disponibles al iniciar:
        if (EstaDesbloqueado("Trompo") || EstaDesbloqueado("Carne") || true) // El "|| true" asegura que se pueda comprar de inmediato
        { 
            Agregar(prefabtrompo, "Trompo", icontrompo); 
        }
        else 
        {
            Debug.Log("¡Trompo bloqueado!");
        }
    }

    public void AñadirLechuga()
    {
        if (EstaDesbloqueado("Lechuga"))
        {
            Agregar(prefablechuga, "Lechuga", iconLechuga);
        }
        else
        {
            Debug.Log("¡Lechuga bloqueada!");
        }
    }

    // Comprobación conectada al ManagerExperiencia
    private bool EstaDesbloqueado(string nombreIngrediente)
    {
        if (ManagerExperiencia.Instancia != null && ManagerExperiencia.Instancia.ingredientesDesbloqueados != null)
        {
            return ManagerExperiencia.Instancia.ingredientesDesbloqueados.Contains(nombreIngrediente);
        }
        return true;
    }

    private void Agregar(GameObject prefab, string nombre, Sprite icono)
    {
        if (prefab == null)
        {
            Debug.LogError("¡El prefab de " + nombre + " no está asignado en el Inspector de TiendaManager!");
            return;
        }

        carritoPrefabs.Add(prefab);
        carritoNombres.Add(nombre);
        carritoIconos.Add(icono);
        Debug.Log("Añadido al carrito: " + nombre);
    }

    public void ComprarCarrito()
    {
        if (carritoPrefabs.Count == 0)
        {
            Debug.Log("El carrito está vacío.");
            return;
        }

        if (prefabCaja == null || puntoEntrega == null)
        {
            Debug.LogError("¡Falta asignar el prefabCaja o el puntoEntrega en el TiendaManager!");
            return;
        }

        for (int i = 0; i < carritoPrefabs.Count; i++)
        {
            Vector3 posicion = puntoEntrega.position + new Vector3(0, i * 0.6f, 0);
            GameObject nuevaCaja = Instantiate(prefabCaja, posicion, Quaternion.identity);

            CajaDelivery infoCaja = nuevaCaja.GetComponent<CajaDelivery>();
            if (infoCaja != null)
            {
                infoCaja.prefabContenido = carritoPrefabs[i];
                infoCaja.nombreContenido = carritoNombres[i];
                infoCaja.iconoContenido = carritoIconos[i];
            }
        }

        Debug.Log("¡Compra realizada con éxito! Cajas generadas.");
        carritoPrefabs.Clear();
        carritoNombres.Clear();
        carritoIconos.Clear();

        CerrarCelular();
    }
}