using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    public Transform camara;
    public float distanciaInteraccion = 15f;
    public ToolManager inventario;
    public TextMeshProUGUI textoAviso;

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
                        textoAviso.text = "Presiona E para colocar el Trompo";
                        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            baseTrompo.ColocarTrompo();
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else
                    {
                        textoAviso.text = "Máquina de Trompo (Vacía)";
                    }
                }
                else
                {
                    if (baseTrompo.usosActuales > 0)
                    {
                        if (inventario.TieneHerramientas()) textoAviso.text = "Manos ocupadas";
                        else
                        {
                            textoAviso.text = "Presiona R para cortar carne (" + baseTrompo.usosActuales + "/7)";
                            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                            {
                                baseTrompo.SacarCarne(inventario);
                            }
                        }
                    }
                    else
                    {
                        if (inventario.TieneHerramientas()) textoAviso.text = "Manos ocupadas";
                        else
                        {
                            textoAviso.text = "Presiona E para quitar trompo vacío";
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
                        textoAviso.text = "Presiona E para llenar la bandeja";
                        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            bandeja.LlenarBandeja();
                            inventario.ConsumirHerramientaActual(); 
                        }
                    }
                    else
                    {
                        textoAviso.text = "Bandeja de Lechuga (Vacía)";
                    }
                }
                else
                {
                    if (inventario.TieneHerramientas())
                    {
                        textoAviso.text = "Manos ocupadas";
                    }
                    else
                    {
                        textoAviso.text = "Presiona R para agarrar lechuga (" + bandeja.porcionesActuales + "/8)";
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
                if (inventario.TieneHerramientas()) textoAviso.text = "Primero suelta tu objeto para agarrar otro";
                else
                {
                    textoAviso.text = "Presiona E para agarrar " + item.nombreHerramienta;

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
                        string ingrediente = inventario.ObtenerNombreActual();
                        textoAviso.text = "Presiona E para agregar " + ingrediente + " al shawarma";

                        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            plato.AgregarIngrediente(ingrediente);
                            inventario.ConsumirHerramientaActual();
                        }
                    }
                    else
                    {
                        textoAviso.text = "El shawarma está lleno";
                    }
                }
                else
                {
                    // Si el plato tiene ingredientes y nuestras manos están vacías
                    if (plato.CantidadIngredientes() > 0)
                    {
                        // Le damos ambas opciones al jugador
                        textoAviso.text = "Q para TIRAR | R para CERRAR shawarma";

                        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
                        {
                            plato.TirarShawarma();
                        }
                        else if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            plato.CerrarShawarma(inventario); // Lo envolvemos y lo agarramos
                        }
                    }
                    else
                    {
                        textoAviso.text = "Pan vacío listo para armar";
                    }
                }
            }

            // CASO E: Miramos a un Cliente
            else if (cliente != null)
            {
                // Si el cliente recién llega
                if (cliente.estadoActual == Cliente.Estado.EsperandoAtencion)
                {
                    
                    textoAviso.text = "Presiona R para aceptar pedido: " + cliente.ObtenerTextoPedido();

                    if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                    {
                        cliente.AceptarPedido();
                        cliente.clienteQueue.UpdatePosition();
                    }
                }
                // Si ya le tomaste el pedido y espera su comida
                else if (cliente.estadoActual == Cliente.Estado.EsperandoComida)
                {
                    if (inventario.ObtenerNombreActual() == "Shawarma")
                    {
                        textoAviso.text = "Presiona R para entregar el Shawarma";

                        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            cliente.RecibirComida(inventario); // Te lo saca de la mano y destruye al cliente
                        }
                    }
                    else
                    {
                        textoAviso.text = "Esperando: " + cliente.ObtenerTextoPedido();
                    }
                }
            }



            // CASO F: Miramos una mesa vacía 
            else
            {
                if (inventario.TieneHerramientas())
                {
                    textoAviso.text = "Presiona E para apoyar";

                    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        Vector3 posicionApoyo = hit.point;
                        inventario.SoltarHerramientaActual(posicionApoyo);
                    }
                }
                else textoAviso.text = "";
            }

        }
        else textoAviso.text = "";

       
    
    
    
    }

}