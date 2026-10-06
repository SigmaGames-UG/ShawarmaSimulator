using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("Física del atropello")]
    public float fuerzaGolpe = 45f;
    public float fuerzaVertical = 15f;
    public float fuerzaGiro = 10f;

    [Header("Caida comica")]
    public float gravedadExtra = 70f;
    public float velocidadMaximaCaida = 35f;

    [Header("Recuperacion")]
    public float tiempoMinimoDerribado = 1f;
    public float velocidadParaLevantarse = 2f;
    public float tiempoParaEnderezarse = 0.6f;

    private Rigidbody rb;
    private CharacterController characterController;
    private CapsuleCollider colliderFisico;
    private Yahya movimiento;

    private bool derribado = false;

    // Guardamos cuándo fue la última vez que tocó una superficie desde arriba
    private float ultimoContactoSuelo = -100f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        colliderFisico = GetComponent<CapsuleCollider>();
        movimiento = GetComponent<Yahya>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (colliderFisico != null)
        {
            colliderFisico.enabled = false;
        }
    }

    public void Morir(Vector3 direccionGolpe)
    {
        if (derribado)
            return;

        derribado = true;

        // Reiniciamos el estado de suelo
        ultimoContactoSuelo = -100f;

        // Apagamos movimiento normal
        if (movimiento != null)
        {
            movimiento.enabled = false;
        }

        // Apagamos CharacterController
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Activamos collider físico
        if (colliderFisico != null)
        {
            colliderFisico.enabled = true;
            colliderFisico.isTrigger = false;
        }

        // Activamos Rigidbody
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.constraints = RigidbodyConstraints.None;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Golpe instantáneo
            Vector3 velocidadGolpe =
                direccionGolpe.normalized * fuerzaGolpe
                + Vector3.up * fuerzaVertical;

            rb.linearVelocity = velocidadGolpe;

            // Giro cómico
            rb.AddTorque(
                Random.insideUnitSphere * fuerzaGiro,
                ForceMode.VelocityChange
            );
        }

        StartCoroutine(Recuperarse());
    }

    private void FixedUpdate()
    {
        if (!derribado || rb == null)
            return;

        // Cuando empieza a caer, agregamos gravedad extra
        if (rb.linearVelocity.y <= 0f)
        {
            rb.AddForce(
                Vector3.down * gravedadExtra,
                ForceMode.Acceleration
            );
        }

        // Limitamos la velocidad máxima hacia abajo
        Vector3 velocidad = rb.linearVelocity;

        if (velocidad.y < -velocidadMaximaCaida)
        {
            velocidad.y = -velocidadMaximaCaida;
            rb.linearVelocity = velocidad;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!derribado)
            return;

        // Revisamos los puntos de contacto
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contacto = collision.GetContact(i);

            // Si la normal apunta hacia arriba,
            // significa que tenemos piso debajo
            if (contacto.normal.y > 0.4f)
            {
                ultimoContactoSuelo = Time.time;
                break;
            }
        }
    }

    private bool EstaEnElSuelo()
    {
        // Consideramos que está en el suelo si hubo
        // contacto con piso hace muy poquito
        return Time.time - ultimoContactoSuelo < 0.15f;
    }

    private IEnumerator Recuperarse()
    {
        // Primero dejamos que ocurra el atropello
        yield return new WaitForSeconds(tiempoMinimoDerribado);

        // PRIMERO esperamos a que realmente toque el piso
        while (!EstaEnElSuelo())
        {
            yield return null;
        }

        // DESPUÉS esperamos a que deje de deslizarse/rebotar
        while (rb != null &&
               rb.linearVelocity.magnitude > velocidadParaLevantarse)
        {
            yield return null;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Nos enderezamos donde realmente caímos
        Quaternion rotacionInicial = transform.rotation;

        float rotacionYActual = transform.eulerAngles.y;

        Quaternion rotacionFinal =
            Quaternion.Euler(
                0f,
                rotacionYActual,
                0f
            );

        float tiempo = 0f;

        while (tiempo < tiempoParaEnderezarse)
        {
            tiempo += Time.deltaTime;

            float porcentaje =
                tiempo / tiempoParaEnderezarse;

            transform.rotation =
                Quaternion.Slerp(
                    rotacionInicial,
                    rotacionFinal,
                    porcentaje
                );

            yield return null;
        }

        transform.rotation = rotacionFinal;

        // Apagamos collider de física
        if (colliderFisico != null)
        {
            colliderFisico.enabled = false;
        }

        // Reactivamos CharacterController
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // Reactivamos movimiento
        if (movimiento != null)
        {
            movimiento.enabled = true;
        }

        derribado = false;
    }
}