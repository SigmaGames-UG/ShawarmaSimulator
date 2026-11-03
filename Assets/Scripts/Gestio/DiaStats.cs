using System;

public class DiaStats : IComparable<DiaStats>
{
    public int numeroDia;
    public float dineroGanado;
    public int shawarmasVendidos;
    public int celebridadesAtendidas;

    public DiaStats(int numeroDia, float dineroGanado, int shawarmasVendidos, int celebridadesAtendidas)
    {
        this.numeroDia = numeroDia;
        this.dineroGanado = dineroGanado;
        this.shawarmasVendidos = shawarmasVendidos;
        this.celebridadesAtendidas = celebridadesAtendidas;
    }

    // Esto le indica al árbol AVL cómo comparar y ordenar los días cronológicamente
    public int CompareTo(DiaStats otro)
    {
        if (otro == null) return 1;
        return this.numeroDia.CompareTo(otro.numeroDia);
    }
}