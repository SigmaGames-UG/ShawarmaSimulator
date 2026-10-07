using UnityEngine;

public class AderezoUsable : MonoBehaviour
{
    [Header("Usos")]
    public int usosTotales = 6;
    public int usosActuales;

    [Header("Efecto visual")]
    public GameObject prefabChorro;
    public Transform puntoSalidaChorro;
    public float duracionChorro = 0.3f;

    private void Start()
    {
        usosActuales = usosTotales;
    }

    public bool Usar()
    {
        if (usosActuales <= 0)
            return false;

        usosActuales--;

        // Creamos el chorrito
        if (prefabChorro != null && puntoSalidaChorro != null)
        {
            GameObject chorro = Instantiate(
                prefabChorro,
                puntoSalidaChorro.position,
                puntoSalidaChorro.rotation
            );

            Destroy(chorro, duracionChorro);
        }

        Debug.Log(
            gameObject.name +
            " - Usos restantes: " +
            usosActuales
        );

        return true;
    }

    public bool EstaVacio()
    {
        return usosActuales <= 0;
    }
}