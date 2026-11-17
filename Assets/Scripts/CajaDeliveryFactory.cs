using UnityEngine;

public class CajaDeliveryFactory : MonoBehaviour
{
    public static GameObject CrearCaja(GameObject prefabCajaBase, Vector3 posicion, GameObject contenidoPrefab, string nombreContenido, Sprite iconoContenido)
    {
        if (prefabCajaBase == null)
        {
            Debug.LogError("¡El prefab base de la caja es nulo en la Factory!");
            return null;
        }

        GameObject nuevaCaja = Instantiate(prefabCajaBase, posicion, Quaternion.identity);

        CajaDelivery infoCaja = nuevaCaja.GetComponent<CajaDelivery>();
        if (infoCaja != null)
        {
            infoCaja.prefabContenido = contenidoPrefab;
            infoCaja.nombreContenido = nombreContenido;
            infoCaja.iconoContenido = iconoContenido;
        }

        return nuevaCaja;
    }
}