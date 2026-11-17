using UnityEngine;

public class DesechoBasura : MonoBehaviour
{
    public void RecogerDesecho()
    {
        Debug.Log("¡Desecho recogido y limpiado!");
        Destroy(gameObject);
    }
}