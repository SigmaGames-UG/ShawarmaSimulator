using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SistemaExperiencia : MonoBehaviour
{
    [Serializable]
    public class ProductoDesbloqueable
    {
        public string nombre;
        public int xpNecesaria;
        public Button botonCompra;

        [NonSerialized] public bool desbloqueado;
    }

    [Header("Experiencia")]
    [SerializeField] private TMP_Text textoExperiencia;
    [SerializeField] private int experienciaPorPulsacion = 25;

    [Header("Productos de la tienda")]
    [SerializeField]
    private ProductoDesbloqueable[] productos =
        new ProductoDesbloqueable[0];

    public int ExperienciaActual { get; private set; }

    private ISimplePriorityQueue<ProductoDesbloqueable> pendientes;

    private void Awake()
    {
        pendientes =
            new SimpleLinkedPriorityQueue<ProductoDesbloqueable>();

        // Cargamos los productos en la cola según la XP que se necesita
        foreach (ProductoDesbloqueable producto in productos)
        {
            producto.desbloqueado = false;

            if (producto.botonCompra != null)
            {
                producto.botonCompra.interactable = false;
            }

            pendientes.Enqueue(producto, producto.xpNecesaria);
        }

        RevisarDesbloqueos();
        ActualizarTexto();
    }

    private void Update()
    {
        // Este boton es de prueba nomas  ya que aun no tenemos el sistema de ventas, en resumen la X suma experiencia.
        if (Keyboard.current != null &&
            Keyboard.current.xKey.wasPressedThisFrame)
        {
            AgregarExperiencia(experienciaPorPulsacion);
        }
    }

    public void AgregarExperiencia(int cantidad)
    {
        if (cantidad <= 0) return;

        ExperienciaActual += cantidad;

        RevisarDesbloqueos();
        ActualizarTexto();
    }

    private void RevisarDesbloqueos()
    {
        // Esto basicamente es para desbloquear los prodcutos que la XP alcanzo
        while (!pendientes.IsEmpty &&
               ExperienciaActual >= pendientes.GetHighestPriority())
        {
            ProductoDesbloqueable producto = pendientes.Dequeue();

            producto.desbloqueado = true;

            if (producto.botonCompra != null)
            {
                producto.botonCompra.interactable = true;
            }

            Debug.Log("Desbloqueaste: " + producto.nombre);
        }
    }

    public bool EstaDesbloqueado(string nombreProducto)
    {
        foreach (ProductoDesbloqueable producto in productos)
        {
            if (producto.nombre == nombreProducto)
            {
                return producto.desbloqueado;
            }
        }

        return false;
    }

    private void ActualizarTexto()
    {
        if (textoExperiencia != null)
        {
            textoExperiencia.text = "XP: " + ExperienciaActual;
        }
    }
}