using TMPro;
using UnityEngine;
using UnityEngine.UI; // ¡Librería necesaria para controlar la Imagen!
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    public Transform camara;
    public float distanciaInteraccion = 15f;
    public ToolManager inventario;

    [Header("Interfaz Visual")]
    public Image iconoInteraccionEnPantalla; // La nueva imagen del Canvas
    public Sprite iconoTeclaE; // Tu asset para la tecla E
    public Sprite iconoTeclaR; // Tu asset para la tecla R
    public Sprite iconoManoBloqueada; // Opcional: Una cruz o mano tachada para "Manos ocupadas"

    public TextMeshProUGUI textoAviso; // Lo conservamos para avisos combinados

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(camara.position, camara.forward, out hit, distanciaInteraccion))
        {
            ItemRecogible item = hit.collider.GetComponentInParent<ItemRecogible>();
            BaseTrompo baseTrompo = hit.collider.GetComponentInParent<BaseTrompo>();
            BandejaLechuga bandeja = hit.collider.GetComponentInParent<BandejaLechuga>();
            ArmadoShawarma plato = hit.collider.GetComponentInParent<ArmadoShawarma>();
            Cliente cliente = hit.collider.GetComponentInParent<Cliente>();

            // CASO A: Miramos la máquina del Trompo
            if (baseTrompo != null)
            {
                if (!baseTrompo.tieneTrompo)
                {
                    if (inventario.ObtenerNombreActual() == "Trompo")
                    {
                        MostrarIcono(iconoTeclaE);
                        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            baseTrompo.ColocarTrompo();
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else MostrarTexto("Máquina de Trompo (Vacía)");
                }
                else
                {
                    if (baseTrompo.usosActuales > 0)
                    {
                        if (inventario.TieneHerramientas()) MostrarIcono(iconoManoBloqueada);
                        else
                        {
                            MostrarIcono(iconoTeclaR);
                            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                            {
                                baseTrompo.SacarCarne(inventario);
                            }
                        }
                    }
                    else
                    {
                        if (inventario.TieneHerramientas()) MostrarIcono(iconoManoBloqueada);
                        else
                        {
                            MostrarIcono(iconoTeclaE);
                            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                            {
                                baseTrompo.LimpiarTrompo();
                            }
                        }
                    }
                }
            }
            // CASO B: Miramos la Bandeja de Lechuga
            else if (bandeja != null)
            {
                if (bandeja.porcionesActuales == 0)
                {
                    if (inventario.ObtenerNombreActual() == "Lechuga")
                    {
                        MostrarIcono(iconoTeclaE);
                        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            bandeja.LlenarBandeja();
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else MostrarTexto("Bandeja de Lechuga (Vacía)");
                }
                else
                {
                    if (inventario.TieneHerramientas()) MostrarIcono(iconoManoBloqueada);
                    else
                    {
                        MostrarIcono(iconoTeclaR);
                        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            bandeja.SacarPorcion(inventario);
                        }
                    }
                }
            }
            // CASO C: Miramos un aderezo, caja, o ítem suelto
            else if (item != null)
            {
                if (inventario.TieneHerramientas()) MostrarIcono(iconoManoBloqueada);
                else
                {
                    MostrarIcono(iconoTeclaE);
                    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        inventario.AgregarHerramienta(item.nombreHerramienta, item.iconoHerramienta, item.gameObject);
                    }
                }
            }
            // CASO D: Miramos el Plato de Armado
            else if (plato != null)
            {
                if (inventario.TieneHerramientas())
                {
                    if (plato.PuedeAgregar())
                    {
                        MostrarIcono(iconoTeclaE);
                        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            plato.AgregarIngrediente(inventario.ObtenerNombreActual());
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else MostrarTexto("El shawarma está lleno");
                }
                else
                {
                    if (plato.CantidadIngredientes() > 0)
                    {
                        MostrarTexto("Q para TIRAR | R para CERRAR");
                        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame) plato.TirarShawarma();
                        else if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) plato.CerrarShawarma(inventario);
                    }
                    else MostrarTexto("Pan vacío listo para armar");
                }
            }
            // CASO E: Miramos a un Cliente
            else if (cliente != null)
            {
                if (cliente.estadoActual == Cliente.Estado.EsperandoAtencion)
                {
                    // Usamos la función combinada para mostrar la tecla y lo que pide
                    MostrarIconoYTexto(iconoTeclaR, "Pedido: " + cliente.ObtenerTextoPedido());
                    if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                    {
                        cliente.AceptarPedido();
                    }
                }
                else if (cliente.estadoActual == Cliente.Estado.EsperandoComida)
                {
                    if (inventario.ObtenerNombreActual() == "Shawarma")
                    {
                        MostrarIconoYTexto(iconoTeclaR, "Entregar Shawarma");
                        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            cliente.RecibirComida(inventario);
                        }
                    }
                    else MostrarTexto("Esperando: " + cliente.ObtenerTextoPedido());
                }
            }
            // CASO F: Miramos una mesa vacía 
            else
            {
                if (inventario.TieneHerramientas())
                {
                    MostrarIcono(iconoTeclaE);
                    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        Vector3 posicionApoyo = hit.point;
                        inventario.SoltarHerramientaActual(posicionApoyo);
                    }
                }
                else OcultarTodo();
            }
        }
        else OcultarTodo();
    }

    // --- FUNCIONES AUXILIARES PARA CONTROLAR LA PANTALLA ---

    void MostrarIcono(Sprite icono)
    {
        if (iconoInteraccionEnPantalla != null)
        {
            iconoInteraccionEnPantalla.sprite = icono;
            iconoInteraccionEnPantalla.enabled = true;
        }
        if (textoAviso != null) textoAviso.text = "";
    }

    void MostrarTexto(string mensaje)
    {
        if (iconoInteraccionEnPantalla != null) iconoInteraccionEnPantalla.enabled = false;
        if (textoAviso != null) textoAviso.text = mensaje;
    }

    void MostrarIconoYTexto(Sprite icono, string mensaje)
    {
        if (iconoInteraccionEnPantalla != null)
        {
            iconoInteraccionEnPantalla.sprite = icono;
            iconoInteraccionEnPantalla.enabled = true;
        }
        if (textoAviso != null) textoAviso.text = mensaje;
    }

    void OcultarTodo()
    {
        if (iconoInteraccionEnPantalla != null) iconoInteraccionEnPantalla.enabled = false;
        if (textoAviso != null) textoAviso.text = "";
    }
}