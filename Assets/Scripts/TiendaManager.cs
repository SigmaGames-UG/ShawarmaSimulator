using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    private ManagerExperiencia managerExperiencia;
    private Vector2[] posicionesIngredientes;
    [SerializeField] private Button botonFiltro;
    [SerializeField] private Button botonOrdenXp;
    [SerializeField] private Button botonOrdenNombre;
    [SerializeField] private GameObject opcionesFiltro;
    private bool ordenarPorNombre;

    private void Start()
    {
        if (panelBotonesTienda == null) return;

        var botones = new List<IngredienteBoton>();
        foreach (Transform hijo in panelBotonesTienda)
        {
            if (hijo.TryGetComponent(out IngredienteBoton boton))
                botones.Add(boton);
        }

        botonesIngredientes = botones.ToArray();
        posicionesIngredientes = new Vector2[botonesIngredientes.Length];
        for (int i = 0; i < botonesIngredientes.Length; i++)
            posicionesIngredientes[i] = ((RectTransform)botonesIngredientes[i].transform).anchoredPosition;

        // The shop uses fixed row positions instead of a layout group.
        CollectionSort.SortArray(posicionesIngredientes, (left, right) => right.y.CompareTo(left.y));
        managerExperiencia = ManagerExperiencia.Instancia;
        if (managerExperiencia != null)
        {
            managerExperiencia.RegistrarDesbloqueosDeTienda(botonesIngredientes);
            managerExperiencia.IngredientesDesbloqueadosCambiaron += ActualizarEstadoBotonesTienda;
        }

        if (botonFiltro != null && opcionesFiltro != null)
            botonFiltro.onClick.AddListener(AlternarOpcionesFiltro);
        if (botonOrdenXp != null)
            botonOrdenXp.onClick.AddListener(OrdenarPorXp);
        if (botonOrdenNombre != null)
            botonOrdenNombre.onClick.AddListener(OrdenarPorNombre);

        ActualizarEstadoBotonesTienda();
    }

    private void OnDestroy()
    {
        if (botonFiltro != null)
            botonFiltro.onClick.RemoveListener(AlternarOpcionesFiltro);
        if (botonOrdenXp != null)
            botonOrdenXp.onClick.RemoveListener(OrdenarPorXp);
        if (botonOrdenNombre != null)
            botonOrdenNombre.onClick.RemoveListener(OrdenarPorNombre);

        if (managerExperiencia != null)
            managerExperiencia.IngredientesDesbloqueadosCambiaron -= ActualizarEstadoBotonesTienda;
    }

    public MonoBehaviour scriptCamara;

    [Header("Bases de Datos, Precios y Salsas")]
    public GameObject prefabKetchup; public int precioKetchup = 15;
    public GameObject prefabtrompo; public int precioCarne = 15;
    public GameObject prefabMayo; public int precioMayo = 20;
    public GameObject prefabTarator; public int precioTarator = 25;
    public GameObject prefablechuga; public int precioLechuga = 10;

    public Sprite iconKetchup;
    public Sprite iconMayo;
    public Sprite iconTarator;
    public Sprite icontrompo;
    public Sprite iconLechuga;

    private List<GameObject> carritoPrefabs = new List<GameObject>();
    private List<string> carritoNombres = new List<string>();
    private List<Sprite> carritoIconos = new List<Sprite>();
    private List<int> carritoPrecios = new List<int>();

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

    private void AlternarOpcionesFiltro()
    {
        opcionesFiltro.SetActive(!opcionesFiltro.activeSelf);
    }

    private void OrdenarPorXp() => SeleccionarOrden(false);

    private void OrdenarPorNombre() => SeleccionarOrden(true);

    private void SeleccionarOrden(bool porNombre)
    {
        ordenarPorNombre = porNombre;
        if (opcionesFiltro != null) opcionesFiltro.SetActive(false);
        OrdenarBotones();
    }

    private void OrdenarBotones()
    {
        if (botonesIngredientes == null) return;

        if (ordenarPorNombre)
        {
            CollectionSort.SortArray(botonesIngredientes,
                (left, right) => StringComparer.CurrentCultureIgnoreCase.Compare(left.nombre, right.nombre));
        }
        else
        {
            CollectionSort.SortArray(botonesIngredientes);
        }

        for (int i = 0; i < botonesIngredientes.Length; i++)
        {
            var rect = (RectTransform)botonesIngredientes[i].transform;
            rect.anchoredPosition = posicionesIngredientes[i];
        }
    }

    public void AbrirTienda()
    {
        if (opcionesFiltro != null) opcionesFiltro.SetActive(false);
        if (panelTienda != null) panelTienda.SetActive(true);
        if (panelCarrito != null) panelCarrito.SetActive(false);
        OrdenarBotones();
        ActualizarEstadoBotonesTienda();
    }

    private void ActualizarEstadoBotonesTienda()
    {
        if (botonesIngredientes == null) return;

        foreach (IngredienteBoton botonIngrediente in botonesIngredientes)
        {
            if (botonIngrediente.TryGetComponent(out Button boton))
                boton.interactable = EstaDesbloqueado(botonIngrediente.nombre);
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

    public void AñadirKetchup()
    {
        if (EstaDesbloqueado("Ketchup"))
        {
            AgregarAlCarrito(prefabKetchup, "Ketchup", iconKetchup, precioKetchup);
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
            AgregarAlCarrito(prefabMayo, "Mayonesa", iconMayo, precioMayo);
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
            AgregarAlCarrito(prefabTarator, "Tarator", iconTarator, precioTarator);
        }
        else
        {
            Debug.Log("¡Tarator bloqueado! Debes desbloquearlo con experiencia primero.");
        }
    }

    public void AñadirTrompo()
    {
        if (EstaDesbloqueado("Trompo") || EstaDesbloqueado("Carne") || true)
        {
            AgregarAlCarrito(prefabtrompo, "Trompo", icontrompo, precioCarne);
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
            AgregarAlCarrito(prefablechuga, "Lechuga", iconLechuga, precioLechuga);
        }
        else
        {
            Debug.Log("¡Lechuga bloqueada!");
        }
    }

    private bool EstaDesbloqueado(string nombreIngrediente)
    {
        if (ManagerExperiencia.Instancia != null && ManagerExperiencia.Instancia.ingredientesDesbloqueados != null)
        {
            return ManagerExperiencia.Instancia.ingredientesDesbloqueados.Contains(nombreIngrediente);
        }
        return true;
    }

    private void AgregarAlCarrito(GameObject prefab, string nombre, Sprite icono, int precio)
    {
        if (prefab == null)
        {
            Debug.LogError("¡El prefab de " + nombre + " no está asignado en el Inspector de TiendaManager!");
            return;
        }

        carritoPrefabs.Add(prefab);
        carritoNombres.Add(nombre);
        carritoIconos.Add(icono);
        carritoPrecios.Add(precio);
        Debug.Log("Añadido al carrito: " + nombre + " ($" + precio + ")");
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

        int costoTotal = 0;
        foreach (int precio in carritoPrecios)
        {
            costoTotal += precio;
        }

        if (ManagerExperiencia.Instancia != null && ManagerExperiencia.Instancia.GastarDinero(costoTotal))
        {
            for (int i = 0; i < carritoPrefabs.Count; i++)
            {
                Vector3 posicion = puntoEntrega.position + new Vector3(0, i * 0.6f, 0);

                GameObject nuevaCaja = CajaDeliveryFactory.CrearCaja(
                    prefabCaja,
                    posicion,
                    carritoPrefabs[i],
                    carritoNombres[i],
                    carritoIconos[i]
                );
            }

            Debug.Log("¡Compra realizada con éxito por $" + costoTotal + "! Cajas generadas mediante Factory.");
            carritoPrefabs.Clear();
            carritoNombres.Clear();
            carritoIconos.Clear();
            carritoPrecios.Clear();

            CerrarCelular();
        }
        else
        {
            Debug.Log("Compra cancelada: No tienes suficiente dinero.");
        }
    }
}