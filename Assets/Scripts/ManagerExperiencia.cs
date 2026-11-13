using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerExperiencia : MonoBehaviour
{
    public static ManagerExperiencia Instancia;

    [Header("Datos de Experiencia y Economía")]
    public int experienciaActual = 0;
    public int dineroActual = 50;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoXpUI;
    public TextMeshProUGUI textoDineroUI;

    [Header("Ingredientes Desbloqueados")]
    public List<string> ingredientesDesbloqueados = new List<string>() { "Carne", "Lechuga", "Trompo" };

    // Variable estática para llevar el control del shawarma que el jugador lleva en la mano
    public static List<string> ingredientesShawarmaEnMano = new List<string>();

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

        InicializarIngredientesBase();
    }

    void Start()
    {
        ActualizarInterfaz();
    }

    public void InicializarIngredientesBase()
    {
        if (ingredientesDesbloqueados == null)
            ingredientesDesbloqueados = new List<string>();

        if (!ingredientesDesbloqueados.Contains("Carne"))
            ingredientesDesbloqueados.Add("Carne");

        if (!ingredientesDesbloqueados.Contains("Lechuga"))
            ingredientesDesbloqueados.Add("Lechuga");

        if (!ingredientesDesbloqueados.Contains("Trompo"))
            ingredientesDesbloqueados.Add("Trompo");
    }

    public void ModificarXP(int cantidad)
    {
        experienciaActual += cantidad;
        if (experienciaActual < 0) experienciaActual = 0;
        ActualizarInterfaz();
    }

    public void RegistrarVentaShawarma(int cantidadIngredientes)
    {
        if (cantidadIngredientes <= 0) cantidadIngredientes = 1;

        int pago = 5 + ((cantidadIngredientes - 1) * 3);
        dineroActual += pago;

        int xpGanada = pago;
        experienciaActual += xpGanada;

        ActualizarInterfaz();
        Debug.Log($"¡Shawarma correcto! Ganaste ${pago} y +{xpGanada} XP.");
    }

    public void RegistrarPedidoIncorrecto(int penalizacionXP = 10)
    {
        experienciaActual -= penalizacionXP;
        if (experienciaActual < 0) experienciaActual = 0;

        ActualizarInterfaz();
        Debug.Log($"¡Pedido incorrecto! No se otorga dinero y se penaliza con -{penalizacionXP} XP.");
    }

    public bool GastarDinero(int costo)
    {
        if (dineroActual >= costo)
        {
            dineroActual -= costo;
            ActualizarInterfaz();
            return true;
        }
        else
        {
            Debug.Log("¡Fondos insuficientes!");
            return false;
        }
    }

    public void DesbloquearIngrediente(string nuevoIngrediente)
    {
        if (!ingredientesDesbloqueados.Contains(nuevoIngrediente))
        {
            ingredientesDesbloqueados.Add(nuevoIngrediente);
        }
    }

    void ActualizarInterfaz()
    {
        if (textoXpUI != null)
            textoXpUI.text = "" + experienciaActual;

        if (textoDineroUI != null)
            textoDineroUI.text = "$" + dineroActual;
    }
}

// Cuando cierras el shawarma, guardamos los ingredientes actuales en la mano del jugador
