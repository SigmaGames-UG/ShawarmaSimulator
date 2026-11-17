using System.Collections.Generic;
using UnityEngine;

public class ClientePool : MonoBehaviour
{
    public static ClientePool Instancia;

    private Dictionary<string, Queue<Cliente>> poolClientes = new Dictionary<string, Queue<Cliente>>();
    private Dictionary<Cliente, string> registroClaves = new Dictionary<Cliente, string>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public Cliente ObtenerCliente(Cliente prefabOriginal, Vector3 posicion, Quaternion rotacion)
    {
        string clave = prefabOriginal.name;

        if (!poolClientes.ContainsKey(clave))
        {
            poolClientes[clave] = new Queue<Cliente>();
        }

        Cliente cliente;

        if (poolClientes[clave].Count > 0)
        {
            cliente = poolClientes[clave].Dequeue();
            cliente.transform.position = posicion;
            cliente.transform.rotation = rotacion;
            cliente.gameObject.SetActive(true);
        }
        else
        {
            cliente = Instantiate(prefabOriginal, posicion, rotacion);
            registroClaves[cliente] = clave;
        }

        return cliente;
    }

    public void DevolverCliente(Cliente cliente)
    {
        if (cliente == null) return;

        cliente.gameObject.SetActive(false);

        string clave = cliente.gameObject.name.Replace("(Clone)", "").Trim();
        if (registroClaves.ContainsKey(cliente))
        {
            clave = registroClaves[cliente];
        }

        if (!poolClientes.ContainsKey(clave))
        {
            poolClientes[clave] = new Queue<Cliente>();
        }

        poolClientes[clave].Enqueue(cliente);
    }
}