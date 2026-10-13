using System;

[System.Serializable]
public class IngredienteInfo : IComparable<IngredienteInfo>
{
    public string nombre;
    public int xpRequerida;

    public IngredienteInfo(string nombre, int xpRequerida)
    {
        this.nombre = nombre;
        this.xpRequerida = xpRequerida;
    }
    public int CompareTo(IngredienteInfo otro)
    {
        return xpRequerida.CompareTo(otro.xpRequerida);
    }
}