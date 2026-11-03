using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ManagerExperiencia : MonoBehaviour
{
    public static ManagerExperiencia Instancia;

    [Header("Datos de Experiencia")]
    public int experienciaActual = 0;
    public TextMeshProUGUI textoXpUI;

    [Header("Ingredientes Desbloqueados")]
    public List<string> ingredientesDesbloqueados = new List<string>() { "Carne", "Lechuga", "Trompo" };
    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Forzamos estrictamente que la lista inicie únicamente con Carne y Lechuga
        InicializarIngredientesBase();
    }

    void Start()
    {
        ActualizarInterfazXP();
    }

    public void InicializarIngredientesBase()
    {
        // Nos aseguramos de que no se dupliquen y contenga solo los dos iniciales
        if (ingredientesDesbloqueados == null)
        {
            ingredientesDesbloqueados = new List<string>();
        }

        // Si la lista está vacía o tiene elementos no deseados al iniciar, la reseteamos
        if (!ingredientesDesbloqueados.Contains("Carne"))
            ingredientesDesbloqueados.Add("Carne");

        if (!ingredientesDesbloqueados.Contains("Lechuga"))
            ingredientesDesbloqueados.Add("Lechuga");
    }

    public void ModificarXP(int cantidad)
    {
        experienciaActual += cantidad;
        if (experienciaActual < 0) experienciaActual = 0;

        ActualizarInterfazXP();
        Debug.Log("Experiencia actualizada: " + experienciaActual);
    }

    // Método para desbloquear nuevos ingredientes mediante la tienda o progresión
    public void DesbloquearIngrediente(string nuevoIngrediente)
    {
        if (!ingredientesDesbloqueados.Contains(nuevoIngrediente))
        {
            ingredientesDesbloqueados.Add(nuevoIngrediente);
            Debug.Log("¡Ingrediente desbloqueado: " + nuevoIngrediente + "!");
        }
    }

    void ActualizarInterfazXP()
    {
        if (textoXpUI != null)
        {
            textoXpUI.text = "XP: " + experienciaActual;
        }
    }
}