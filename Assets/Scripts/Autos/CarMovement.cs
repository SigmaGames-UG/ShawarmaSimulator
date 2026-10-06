using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Transform destino;
    private float velocidad;

    public void Inicializar(
        Transform nuevoDestino,
        float nuevaVelocidad)
    {
        destino = nuevoDestino;
        velocidad = nuevaVelocidad;
    }

    private void Update()
    {
        if (destino == null)
            return;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                destino.position,
                velocidad * Time.deltaTime
            );

        if (Vector3.Distance(
            transform.position,
            destino.position) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerDeath jugador =
            other.GetComponentInParent<PlayerDeath>();

        if (jugador != null)
        {
            Vector3 direccionGolpe =
                jugador.transform.position
                - transform.position;

            direccionGolpe.y = 0f;

            jugador.Morir(direccionGolpe);
        }


    }
}