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
    public List<string> ingredientesDesbloqueados = new List<string>() { "Carne", "Lechuga" };

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

        
    }


    public void ModificarXP(int cantidad)
    {
        int xpAnterior = experienciaActual;

        experienciaActual += cantidad;

        if (experienciaActual < 0)
            experienciaActual = 0;

        int cambioReal = experienciaActual - xpAnterior;

        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarCambioXP(cambioReal);
        }

        ActualizarInterfaz();
    }




    public void RegistrarVentaShawarma(int cantidadIngredientes)
    {
        if (cantidadIngredientes <= 0)
            cantidadIngredientes = 1;

        int pago = 5 + ((cantidadIngredientes - 1) * 3);

        dineroActual += pago;

        int xpGanada = pago;
        ModificarXP(xpGanada);

        if (DayManager.Instancia != null)
        {
            DayManager.Instancia.RegistrarVenta(pago);
        }

        ActualizarInterfaz();

        Debug.Log($"Venta correcta: +${pago} y +{xpGanada} XP");
    }




    public void RegistrarPedidoIncorrecto(int penalizacionXP = 10)
    {
        ModificarXP(-Mathf.Abs(penalizacionXP));

        Debug.Log(
            "Pedido incorrecto. Sin dinero y con penalización de XP."
        );
    }



    public bool GastarDinero(int costo)
    {
        if (costo < 0)
            return false;

        if (dineroActual >= costo)
        {
            dineroActual -= costo;

            if (DayManager.Instancia != null)
            {
                DayManager.Instancia.RegistrarGasto(costo);
            }

            ActualizarInterfaz();

            Debug.Log("Compra realizada: -$" + costo);

            return true;
        }

        Debug.Log("Fondos insuficientes");
        return false;
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

