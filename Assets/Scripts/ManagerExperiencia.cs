using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ManagerExperiencia : MonoBehaviour
{
    public static ManagerExperiencia Instancia;

    [Serializable]
    public class ProductoDesbloqueable
    {
        public string nombre;
        public int xpNecesaria;
        public Button botonCompra;

        [NonSerialized] public bool desbloqueado;
    }

    [Header("Experiencia")]
    public int XP = 0;
    [SerializeField] private TMP_Text textoXP;

    [Header("Productos de la tienda (Celular)")]
    [SerializeField] private ProductoDesbloqueable[] productos = new ProductoDesbloqueable[0];

    SimpleArrayList<IngredienteInfo> ingredienteList = new SimpleArrayList<IngredienteInfo>();
    IngredienteInfo[] ingredienteArray;
    public ISimpleSet<string> ingredientesDesbloqueados = new SimpleArraySet<string>(10);
    private ISimplePriorityQueue<string> ingredientesBloqueados = new SimpleArrayPriorityQueue<string>();

    private ISimplePriorityQueue<ProductoDesbloqueable> pendientesCelular;

    private AVL<string> catalogoAVL = new AVL<string>();

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Inicializamos sets y AVL base
        ingredientesDesbloqueados.Add("Carne");
        ingredientesDesbloqueados.Add("Lechuga");
        catalogoAVL.Insert("Carne");
        catalogoAVL.Insert("Lechuga");

        ingredientesBloqueados.Enqueue("Ketchup", 50);
        ingredientesBloqueados.Enqueue("Mostaza", 100);
        ingredientesBloqueados.Enqueue("Mayonesa", 150);

        // Inicializamos la cola del celular
        pendientesCelular = new SimpleLinkedPriorityQueue<ProductoDesbloqueable>();

        foreach (ProductoDesbloqueable producto in productos)
        {
            producto.desbloqueado = false;
            if (producto.botonCompra != null)
            {
                producto.botonCompra.interactable = false;
            }
            pendientesCelular.Enqueue(producto, producto.xpNecesaria);
        }

        ingredienteList.Add(new IngredienteInfo("Mayonesa", 150));
        ingredienteList.Add(new IngredienteInfo("Ketchup", 50));
        ingredienteList.Add(new IngredienteInfo("Mostaza", 100));
        ingredienteArray = ingredienteList.ToArray();

        CollectionSort.SortArray(ingredienteArray);

        ActualizarPantallaXP();

        // Evaluamos según la XP actual (si es 0, mantendrá todo bloqueado)
        RevisarDesbloqueos();
    }

    public void ModificarXP(int cantidad)
    {
        if (cantidad == 0) return;

        XP += cantidad;
        if (XP < 0) XP = 0;

        Debug.Log("XP Actualizada: " + XP);

        ActualizarPantallaXP();
        RevisarDesbloqueos();
    }

    private void ActualizarPantallaXP()
    {
        if (textoXP != null)
        {
            textoXP.text = "XP: " + XP;
        }
    }

    private void RevisarDesbloqueos()
    {
        while (!ingredientesBloqueados.IsEmpty && XP >= ingredientesBloqueados.GetHighestPriority())
        {
            string nuevoIngrediente = ingredientesBloqueados.Dequeue();
            ingredientesDesbloqueados.Add(nuevoIngrediente);
            catalogoAVL.Insert(nuevoIngrediente);

            Debug.Log("¡NUEVO INGREDIENTE DESBLOQUEADO Y REGISTRADO EN AVL: " + nuevoIngrediente + "!");
        }

        while (!pendientesCelular.IsEmpty && XP >= pendientesCelular.GetHighestPriority())
        {
            ProductoDesbloqueable producto = pendientesCelular.Dequeue();
            producto.desbloqueado = true;

            if (producto.botonCompra != null)
            {
                producto.botonCompra.interactable = true;
            }

            Debug.Log("Desbloqueaste en el celular: " + producto.nombre);
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

    public bool IngredienteEstaDisponible(string nombreIngrediente)
    {
        return catalogoAVL.Search(nombreIngrediente);
    }
}