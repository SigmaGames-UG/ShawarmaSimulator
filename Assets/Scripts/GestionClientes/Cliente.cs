using UnityEngine;

public class Cliente : MonoBehaviour
{
    public enum Estado { EsperandoAtencion, EsperandoComida, Satisfecho }
    public Estado estadoActual = Estado.EsperandoAtencion;
    private bool moving = false;
    private int speed = 140;
    private Vector3 targetPosition;

    
    public ISimpleSet<string> pedido = new SimpleArraySet<string>(5);
    public ClienteQueue clienteQueue;

    private string[] ingredientesPosibles = { "Carne", "Lechuga", "Ketchup" };

    void Start()
    {
        GenerarPedido();
    }

    void GenerarPedido()
    {
       
        pedido.Add("Carne");

        
        int cantidadDeseada = Random.Range(1, 4);

        
        while (pedido.Count < cantidadDeseada)
        {
            string ingredienteAzar = ingredientesPosibles[Random.Range(0, ingredientesPosibles.Length)];

            pedido.Add(ingredienteAzar);
        }
    }

    public string ObtenerTextoPedido()
    {
        return string.Join(" + ", pedido.ToArray());
    }

    public void AceptarPedido()
    {
        estadoActual = Estado.EsperandoComida;
    }

    public void RecibirComida(ToolManager inventario)
    {
        inventario.ConsumirHerramientaActual();
        estadoActual = Estado.Satisfecho;
        Destroy(gameObject);
    }

    private void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        if (transform.position == targetPosition)
        {
            moving = false;
        }
    }

    public void MoveTo(Vector3 Position)
    {
        targetPosition = Position;
        moving = true;
    }
}