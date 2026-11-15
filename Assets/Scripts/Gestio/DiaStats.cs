
using System;

public class DiaStats : IComparable<DiaStats>
{
    public int numeroDia;

    public int clientesAtendidos;
    public int shawarmasVendidos;
    public int celebridadesAtendidas;

    public float dineroGanado;
    public float dineroGastado;

    public int xpGanada;
    public int xpPerdida;

    public int PedidosIncorrectos =>
        Math.Max(0, clientesAtendidos - shawarmasVendidos);

    public float BalanceNeto =>
        dineroGanado - dineroGastado;

    public int BalanceXP =>
        xpGanada - xpPerdida;

    public DiaStats(
        int numeroDia,
        int clientesAtendidos,
        int shawarmasVendidos,
        int celebridadesAtendidas,
        float dineroGanado,
        float dineroGastado,
        int xpGanada,
        int xpPerdida)
    {
        this.numeroDia = numeroDia;
        this.clientesAtendidos = clientesAtendidos;
        this.shawarmasVendidos = shawarmasVendidos;
        this.celebridadesAtendidas = celebridadesAtendidas;

        this.dineroGanado = dineroGanado;
        this.dineroGastado = dineroGastado;

        this.xpGanada = xpGanada;
        this.xpPerdida = xpPerdida;
    }

    public int CompareTo(DiaStats otro)
    {
        if (otro == null)
            return 1;

        return numeroDia.CompareTo(otro.numeroDia);
    }
}
