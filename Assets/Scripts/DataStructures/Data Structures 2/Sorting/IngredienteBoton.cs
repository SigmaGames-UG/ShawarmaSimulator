using UnityEngine;

public class IngredienteBoton : MonoBehaviour,
    System.IComparable<IngredienteBoton>
{
    public string nombre;
    public int xpRequerida;

    public int CompareTo(IngredienteBoton otro)
    {
        return xpRequerida.CompareTo(otro.xpRequerida);
    }
}