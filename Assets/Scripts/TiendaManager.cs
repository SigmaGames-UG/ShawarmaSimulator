using System;

using System.Collections.Generic;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;

using UnityEngine.InputSystem;

using UnityEngine.UI;

using static System.Net.Mime.MediaTypeNames;



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

        ActualizarEstadoBotonesTienda();

    }



    private void ActualizarEstadoBotonesTienda()

    {

        if (botonesIngredientes == null) return;



        foreach (var botonIngrediente in botonesIngredientes)

        {

            if (botonIngrediente != null)

            {

                TextMeshProUGUI textoBoton = botonIngrediente.GetComponentInChildren<TextMeshProUGUI>();



                if (textoBoton != null)

                {

                    string nombreIngrediente = textoBoton.text;

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



    // --- MÉTODOS DE COMPRA CON PRECIOS Y VALIDACIÓN ---

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



        // Sumar el costo total de los productos en el carrito

        int costoTotal = 0;

        foreach (int precio in carritoPrecios)

        {

            costoTotal += precio;

        }



        // Intentar descontar el dinero mediante el ManagerExperiencia

        if (ManagerExperiencia.Instancia != null && ManagerExperiencia.Instancia.GastarDinero(costoTotal))

        {

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



            Debug.Log("¡Compra realizada con éxito por $" + costoTotal + "! Cajas generadas.");

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