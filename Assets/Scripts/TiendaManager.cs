using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TiendaManager : MonoBehaviour
{
    public GameObject uiCelular;
    public GameObject panelTienda;
    public GameObject panelCarrito;
    public Transform puntoEntrega;
    public GameObject prefabCaja;

    // ¡NUEVA VARIABLE! Acá arrastraremos el script que mueve tu visión
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

                // Apagamos el script de la cámara para que no gire
                if (scriptCamara != null) scriptCamara.enabled = false;
            }
            else
            {
                CerrarCelular();
            }
        }
    }

    public void AbrirTienda() { panelTienda.SetActive(true); panelCarrito.SetActive(false); }
    public void AbrirCarrito() { panelTienda.SetActive(false); panelCarrito.SetActive(true); }

    public void CerrarCelular()
    {
        uiCelular.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Volvemos a prender el script de la cámara
        if (scriptCamara != null) scriptCamara.enabled = true;
    }

    public void AñadirKetchup() { Agregar(prefabKetchup, "Ketchup", iconKetchup); }
    public void AñadirMayo() { Agregar(prefabMayo, "Mayonesa", iconMayo); }
    public void AñadirTarator() { Agregar(prefabTarator, "Tarator", iconTarator); }
    public void AñadirTrompo() { Agregar(prefabtrompo, "Trompo", icontrompo); }

    public void AñadirLechuga() { Agregar(prefablechuga, "Lechuga", iconLechuga); }

    private void Agregar(GameObject prefab, string nombre, Sprite icono)
    {
        carritoPrefabs.Add(prefab);
        carritoNombres.Add(nombre);
        carritoIconos.Add(icono);
    }

    public void ComprarCarrito()
    {
        if (carritoPrefabs.Count == 0) return;

        for (int i = 0; i < carritoPrefabs.Count; i++)
        {
            Vector3 posicion = puntoEntrega.position + new Vector3(0, i * 0.6f, 0);
            GameObject nuevaCaja = Instantiate(prefabCaja, posicion, Quaternion.identity);

            CajaDelivery infoCaja = nuevaCaja.GetComponent<CajaDelivery>();
            infoCaja.prefabContenido = carritoPrefabs[i];
            infoCaja.nombreContenido = carritoNombres[i];
            infoCaja.iconoContenido = carritoIconos[i];
        }

        carritoPrefabs.Clear();
        carritoNombres.Clear();
        carritoIconos.Clear();
    }
}