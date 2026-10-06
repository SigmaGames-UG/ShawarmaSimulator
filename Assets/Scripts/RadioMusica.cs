using UnityEngine;

public class RadioMusica : MonoBehaviour
{
    [Header("Estaciones")]
    public AudioClip[] estaciones;

    private AudioSource audioSource;
    private int estacionActual = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // La radio siempre empieza apagada
        audioSource.playOnAwake = false;
        audioSource.Stop();
    }

    public bool EstaEncendida()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    public void EncenderOCambiarEstacion()
    {
        if (estaciones.Length == 0)
            return;

        // Si está apagada, la prendemos en la estación actual
        if (!audioSource.isPlaying)
        {
            audioSource.clip = estaciones[estacionActual];
            audioSource.Play();

            Debug.Log("Radio encendida");
            return;
        }

        // Si ya estaba prendida, cambiamos de estación
        estacionActual++;

        if (estacionActual >= estaciones.Length)
        {
            estacionActual = 0;
        }

        audioSource.clip = estaciones[estacionActual];
        audioSource.Play();

        Debug.Log("Estación actual: " + estacionActual);
    }

    public void Apagar()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();

        Debug.Log("Radio apagada");
    }
}