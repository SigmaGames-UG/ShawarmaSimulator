using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogrosManager : MonoBehaviour
{
    public static LogrosManager Instancia;

    private ISimpleSet<string> logrosDesbloqueados = new SimpleArraySet<string>();

    [Header("UI de Logros (Imagen en Pantalla)")]
    public Image imagenLogroUI;
    public float tiempoVisible = 3f;

    [Header("Audio de Logros")]
    public AudioSource audioSource;
    public AudioClip sonidoLogro;

    [System.Serializable]
    public struct LogroVisual
    {
        public string idLogro;
        public Sprite iconoLogro;
    }

    [Header("Lista de Logros e Imágenes asociadas")]
    public List<LogroVisual> catalogoLogros = new List<LogroVisual>();
    private Dictionary<string, Sprite> diccionarioLogros = new Dictionary<string, Sprite>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        foreach (var logro in catalogoLogros)
        {
            if (!diccionarioLogros.ContainsKey(logro.idLogro))
            {
                diccionarioLogros.Add(logro.idLogro, logro.iconoLogro);
            }
        }
    }

    void Start()
    {
        if (imagenLogroUI != null)
        {
            imagenLogroUI.gameObject.SetActive(false);
        }
    }

    public void DesbloquearLogro(string idLogro)
    {
        if (!logrosDesbloqueados.Contains(idLogro))
        {
            logrosDesbloqueados.Add(idLogro);
            Debug.Log("¡Logro desbloqueado: " + idLogro + "!");

            MostrarNotificacionLogro(idLogro);
        }
    }

    void MostrarNotificacionLogro(string idLogro)
    {
        if (sonidoLogro != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoLogro);
        }

        if (diccionarioLogros.ContainsKey(idLogro) && imagenLogroUI != null)
        {
            imagenLogroUI.sprite = diccionarioLogros[idLogro];
            StopAllCoroutines();
            StartCoroutine(RutinaMostrarImagen());
        }
    }

    System.Collections.IEnumerator RutinaMostrarImagen()
    {
        imagenLogroUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(tiempoVisible);
        imagenLogroUI.gameObject.SetActive(false);
    }
}