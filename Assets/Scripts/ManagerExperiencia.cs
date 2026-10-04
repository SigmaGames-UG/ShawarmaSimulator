using UnityEngine;
using TMPro; // ¡Agregamos esto para poder editar los textos de la interfaz!

public class ManagerExperiencia : MonoBehaviour
{
    public static ManagerExperiencia Instancia;

    public int XP = 0;

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoXP; // El casillero para arrastrar tu texto del Canvas

    public ISimpleSet<string> ingredientesDesbloqueados = new SimpleArraySet<string>(10);
    private ISimplePriorityQueue<string> ingredientesBloqueados = new SimpleArrayPriorityQueue<string>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
    }

    void Start()
    {
        ingredientesDesbloqueados.Add("Carne");
        ingredientesDesbloqueados.Add("Lechuga");

        ingredientesBloqueados.Enqueue("Ketchup", 50);
        ingredientesBloqueados.Enqueue("Mostaza", 100);
        ingredientesBloqueados.Enqueue("Mayonesa", 150);

        ActualizarPantallaXP(); // Setea el texto en 0 al arrancar
    }

    public void ModificarXP(int cantidad)
    {
        XP += cantidad;
        if (XP < 0) XP = 0; // Evita que la XP baje de cero

        Debug.Log("XP Actualizada: " + XP);

        ActualizarPantallaXP(); // ¡Refresca el texto visual inmediatamente!
        RevisarDesbloqueos();
    }

    private void ActualizarPantallaXP()
    {
        // Si asignaste el texto en el Inspector, lo actualiza
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
            Debug.Log("¡NUEVO INGREDIENTE DESBLOQUEADO: " + nuevoIngrediente + "!");
        }
    }
}