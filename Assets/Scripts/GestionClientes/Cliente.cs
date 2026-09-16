using UnityEngine;
using System.Collections.Generic;

public class Cliente : MonoBehaviour
{
    public enum Estado { EsperandoAtencion, EsperandoComida, Satisfecho }
    public Estado estadoActual = Estado.EsperandoAtencion;

    // Usamos una Lista para guardar los ingredientes que quiere
    public List<string> pedido = new List<string>();

    // Lista de ingredientes posibles 
    private string[] ingredientesPosibles = { "Carne", "Lechuga", "Ketchup" };

    void Start()
    {
        GenerarPedido();
    }

    void GenerarPedido()
    {
        // siempre pide Carne de forma fija.
        pedido.Add("Carne");

        /* 
        // PARA HACERLO RANDOM CUANDO SOL ME PASE MAS ASSETS, DESCOMENTA ESTO:
        int cantidadIngredientes = Random.Range(1, 4); // Pide entre 1 y 3 cosas
        for (int i = 0; i < cantidadIngredientes; i++)
        {
            string ingredienteAzar = ingredientesPosibles[Random.Range(0, ingredientesPosibles.Length)];
            pedido.Add(ingredienteAzar);
        }
        */
    }

    
    public string ObtenerTextoPedido()
    {
        return string.Join(" + ", pedido);
    }

    public void AceptarPedido()
    {
        estadoActual = Estado.EsperandoComida;
    }

    public void RecibirComida(ToolManager inventario)
    {
        inventario.ConsumirHerramientaActual(); 
        estadoActual = Estado.Satisfecho;

        // El cliente se va (Por ahora lo destruimos)
        Destroy(gameObject);
    }
}