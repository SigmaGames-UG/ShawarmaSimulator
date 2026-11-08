using UnityEngine;

public class ItemRecogible : MonoBehaviour
{
    public string nombreHerramienta;
    public Sprite iconoHerramienta;

    [Header("Posicion en la mano")]
    public Vector3 posicionEnMano = new Vector3(0,0,0);
    public Vector3 rotacionEnMano;
}