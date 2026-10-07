using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    public Transform camara;
    public float distanciaInteraccion = 15f;
    public ToolManager inventario;

    [Header("Interfaz Visual")]
    public Image iconoInteraccionEnPantalla;
    public Sprite iconoTeclaE;
    public Sprite iconoTeclaR;
    public Sprite iconoManoBloqueada;

    public TextMeshProUGUI textoAviso;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            camara.position,
            camara.forward,
            out hit,
            distanciaInteraccion))
        {
            ItemRecogible item =
                hit.collider.GetComponentInParent<ItemRecogible>();

            BaseTrompo baseTrompo =
                hit.collider.GetComponentInParent<BaseTrompo>();

            BandejaLechuga bandeja =
                hit.collider.GetComponentInParent<BandejaLechuga>();

            ArmadoShawarma plato =
                hit.collider.GetComponentInParent<ArmadoShawarma>();

            Cliente cliente =
                hit.collider.GetComponentInParent<Cliente>();

            RadioMusica radio =
                hit.collider.GetComponentInParent<RadioMusica>();

            // CASO A: MÁQUINA DEL TROMPO

            if (baseTrompo != null)
            {
                if (!baseTrompo.tieneTrompo)
                {
                    if (inventario.ObtenerNombreActual() == "Trompo")
                    {
                        MostrarIconoYTexto(
                            iconoTeclaE,
                            "Colocar Trompo de carne"
                        );

                        if (Keyboard.current != null &&
                            Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            baseTrompo.ColocarTrompo();
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else
                    {
                        MostrarTexto(
                            "Máquina vacía (Necesitas un bloque de carne)"
                        );
                    }
                }
                else
                {
                    if (baseTrompo.usosActuales > 0)
                    {
                        if (inventario.TieneHerramientas())
                        {
                            MostrarIcono(iconoManoBloqueada);
                        }
                        else
                        {
                            MostrarIconoYTexto(
                                iconoTeclaR,
                                "Cortar Carne (" +
                                baseTrompo.usosActuales +
                                "/7)"
                            );

                            if (Keyboard.current != null &&
                                Keyboard.current.rKey.wasPressedThisFrame)
                            {
                                baseTrompo.SacarCarne(inventario);
                            }
                        }
                    }
                    else
                    {
                        if (inventario.TieneHerramientas())
                        {
                            MostrarIcono(iconoManoBloqueada);
                        }
                        else
                        {
                            MostrarIconoYTexto(
                                iconoTeclaE,
                                "Falta Carne. Presiona E para quitar el fierro"
                            );

                            if (Keyboard.current != null &&
                                Keyboard.current.eKey.wasPressedThisFrame)
                            {
                                baseTrompo.LimpiarTrompo();
                            }
                        }
                    }
                }
            }

            // CASO B: BANDEJA DE LECHUGA

            else if (bandeja != null)
            {
                if (bandeja.porcionesActuales == 0)
                {
                    if (inventario.ObtenerNombreActual() == "Lechuga")
                    {
                        MostrarIcono(iconoTeclaE);

                        if (Keyboard.current != null &&
                            Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            bandeja.LlenarBandeja();
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else
                    {
                        MostrarTexto(
                            "Bandeja de Lechuga (Vacía)"
                        );
                    }
                }
                else
                {
                    if (inventario.TieneHerramientas())
                    {
                        MostrarIcono(iconoManoBloqueada);
                    }
                    else
                    {
                        MostrarIcono(iconoTeclaR);

                        if (Keyboard.current != null &&
                            Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            bandeja.SacarPorcion(inventario);
                        }
                    }
                }
            }


            // CASO C: RADIO
            

            else if (radio != null)
            {
                if (!radio.EstaEncendida())
                {
                    MostrarIconoYTexto(
                        iconoTeclaE,
                        "Encender radio"
                    );

                    if (Keyboard.current != null &&
                        Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        radio.EncenderOCambiarEstacion();
                    }
                }
                else
                {
                    MostrarIconoYTexto(
                        iconoTeclaE,
                        "E: Cambiar estación | T: Apagar"
                    );

                    // E cambia de estación
                    if (Keyboard.current != null &&
                        Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        radio.EncenderOCambiarEstacion();
                    }

                    // T apaga la radio
                    if (Keyboard.current != null &&
                        Keyboard.current.tKey.wasPressedThisFrame)
                    {
                        radio.Apagar();
                    }
                }
            }
            // CASO D: ITEM RECOGIBLE

            else if (item != null)
            {
                if (inventario.TieneHerramientas())
                {
                    MostrarIcono(iconoManoBloqueada);
                }
                else
                {
                    MostrarIcono(iconoTeclaE);

                    if (Keyboard.current != null &&
                        Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        inventario.AgregarHerramienta(
                            item.nombreHerramienta,
                            item.iconoHerramienta,
                            item.gameObject
                        );
                    }
                }
            }

            // CASO E: PLATO DE ARMADO

            else if (plato != null)
            {
                if (inventario.TieneHerramientas())
                {
                    if (plato.PuedeAgregar())
                    {
                        string ingrediente =
                            inventario.ObtenerNombreActual();

                        MostrarIconoYTexto(
                            iconoTeclaE,
                            "Agregar " +
                            ingrediente +
                            " al shawarma"
                        );

                        if (Keyboard.current != null &&
                            Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            plato.AgregarIngrediente(ingrediente);

                            inventario.UsarHerramientaActual();
                        }
                    }
                    else
                    {
                        MostrarTexto(
                            "El shawarma está lleno"
                        );
                    }
                }
                else
                {
                    if (plato.CantidadIngredientes() > 0)
                    {
                        MostrarTexto(
                            "Q para TIRAR | R para CERRAR"
                        );

                        if (Keyboard.current != null &&
                            Keyboard.current.qKey.wasPressedThisFrame)
                        {
                            plato.TirarShawarma();
                        }
                        else if (
                            Keyboard.current != null &&
                            Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            plato.CerrarShawarma(inventario);
                        }
                    }
                    else
                    {
                        MostrarTexto(
                            "Pan vacío listo para armar"
                        );
                    }
                }
            }


            // CASO F: CLIENTE


            else if (cliente != null)
            {
                if (cliente.estadoActual ==
                    Cliente.Estado.EsperandoAtencion)
                {
                    MostrarIconoYTexto(
                        iconoTeclaR,
                        "Pedido: " +
                        cliente.ObtenerTextoPedido()
                    );

                    if (Keyboard.current != null &&
                        Keyboard.current.rKey.wasPressedThisFrame)
                    {
                        cliente.AceptarPedido();
                    }
                }
                else if (
                    cliente.estadoActual ==
                    Cliente.Estado.EsperandoComida)
                {
                    if (inventario.ObtenerNombreActual() == "Shawarma")
                    {
                        MostrarIconoYTexto(
                            iconoTeclaR,
                            "Entregar Shawarma"
                        );

                        if (Keyboard.current != null &&
                            Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            cliente.RecibirComida(inventario);
                        }
                    }
                    else
                    {
                        MostrarTexto(
                            "Esperando: " +
                            cliente.ObtenerTextoPedido()
                        );
                    }
                }
            }


          
            // CASO G: SUPERFICIE
          

            else
            {
                if (inventario.TieneHerramientas())
                {
                    // Mesa o piso
                    if (hit.normal.y > 0.7f)
                    {
                        MostrarIconoYTexto(
                            iconoTeclaE,
                            "Apoyar"
                        );

                        if (Keyboard.current != null &&
                            Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            Vector3 posicionApoyo =
                                hit.point +
                                new Vector3(0, 0.2f, 0);

                            inventario.SoltarHerramientaActual(
                                posicionApoyo
                            );
                        }
                    }

                    // Pared o techo
                    else
                    {
                        MostrarTexto(
                            "No puedes apoyar esto en la pared"
                        );
                    }
                }
                else
                {
                    OcultarTodo();
                }
            }
        }
        else
        {
            OcultarTodo();
        }
    }
    
    
    // FUNCIONES AUXILIARES DE INTERFAZ
    

    void MostrarIcono(Sprite icono)
    {
        if (iconoInteraccionEnPantalla != null)
        {
            iconoInteraccionEnPantalla.sprite = icono;
            iconoInteraccionEnPantalla.enabled = true;
        }

        if (textoAviso != null)
        {
            textoAviso.text = "";
        }
    }


    void MostrarTexto(string mensaje)
    {
        if (iconoInteraccionEnPantalla != null)
        {
            iconoInteraccionEnPantalla.enabled = false;
        }

        if (textoAviso != null)
        {
            textoAviso.text = mensaje;
        }
    }


    void MostrarIconoYTexto(
        Sprite icono,
        string mensaje)
    {
        if (iconoInteraccionEnPantalla != null)
        {
            iconoInteraccionEnPantalla.sprite = icono;
            iconoInteraccionEnPantalla.enabled = true;
        }

        if (textoAviso != null)
        {
            textoAviso.text = mensaje;
        }
    }


    void OcultarTodo()
    {
        if (iconoInteraccionEnPantalla != null)
        {
            iconoInteraccionEnPantalla.enabled = false;
        }

        if (textoAviso != null)
        {
            textoAviso.text = "";
        }
    }
}