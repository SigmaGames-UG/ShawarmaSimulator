using UnityEngine;
using System.Collections.Generic;
using TMPro; //

public class ClienteSpawner : MonoBehaviour
{
    public GameObject prefabCliente;
    public Transform[] puntosDeSpawn;
    public float tiempoEntreClientes = 15f;
    private float timer = 0f;

    public List<GameObject> clientesActivos = new List<GameObject>();

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoPedidosUI; 

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tiempoEntreClientes)
        {
            SpawnearCliente();
            timer = 0f;
        }

       
        clientesActivos.RemoveAll(item => item == null);

        ActualizarPantallaPedidos();
    }

    void SpawnearCliente()
    {
        if (puntosDeSpawn.Length == 0) return;

       
        List<Transform> puntosLibres = new List<Transform>();

        foreach (Transform punto in puntosDeSpawn)
        {
            bool ocupado = false;
            foreach (GameObject cliente in clientesActivos)
            {
                
                if (cliente != null && Vector3.Distance(cliente.transform.position, punto.position) < 1.0f)
                {
                    ocupado = true;
                    break;
                }
            }
            if (!ocupado) puntosLibres.Add(punto);
        }

       
        if (puntosLibres.Count == 0) return;

        
        Transform puntoElegido = puntosLibres[Random.Range(0, puntosLibres.Count)];

        GameObject nuevoCliente = Instantiate(prefabCliente, puntoElegido.position, puntoElegido.rotation);
        clientesActivos.Add(nuevoCliente);
    }

    void ActualizarPantallaPedidos()
    {
        if (textoPedidosUI == null) return;

        string textoFinal = "ÓRDENES ACTIVAS:\n\n";
        int ordenesPendientes = 0;

        foreach (GameObject obj in clientesActivos)
        {
            if (obj != null)
            {
                Cliente cliente = obj.GetComponent<Cliente>();
             
                if (cliente != null && cliente.estadoActual == Cliente.Estado.EsperandoComida)
                {
                    textoFinal += "- Shawarma (" + cliente.ObtenerTextoPedido() + ")\n";
                    ordenesPendientes++;
                }
            }
        }

        if (ordenesPendientes == 0) textoFinal += "Sin pedidos por ahora...";

        textoPedidosUI.text = textoFinal;
    }
}