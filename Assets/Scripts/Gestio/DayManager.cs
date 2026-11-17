
using UnityEngine;
using TMPro;
using System.Text;

public class DayManager : MonoBehaviour
{
    public static DayManager Instancia;

    [Header("Interfaz")]
    public TextMeshProUGUI textoRelojUnico;
    public TextMeshProUGUI textoBalanceUI;
    public GameObject panelEstadisticas;
    private bool panelAbierto = false;

    [Header("Tiempo")]
    public int horaActual = 11;
    public int minutoActual = 0;

    private bool localAbierto = false;
    private bool esperandoCierre = false;
    private float acumuladorTiempo = 0f;

    private const float segundosPorMinutoJuego = 0.25f;

    public bool EstaAbierto => localAbierto;

    [Header("Día")]
    public int diaActual = 1;

    [Header("Estadísticas Diarias")]
    public int clientesAtendidosHoy = 0;
    public int shawarmasHoy = 0;
    public int celebridadesHoy = 0;

    public float dineroHoy = 0f;
    public float dineroGastadoHoy = 0f;

    public int xpGanadaHoy = 0;
    public int xpPerdidaHoy = 0;

    private AVL<DiaStats> historialDiasAVL =
        new AVL<DiaStats>();

    void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (textoRelojUnico != null)
            textoRelojUnico.text = "El día aún no comienza";

        if (textoBalanceUI != null)
            textoBalanceUI.text = "";
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelAbierto)
                CerrarPanelEstadisticas();
            else
                MostrarEstadisticasEnPausa();

            return;
        }

        if (panelAbierto) return;

        if (!localAbierto && !esperandoCierre &&
            Input.GetKeyDown(KeyCode.G))
        {
            AbrirLocal();
        }

        if (localAbierto)
        {
            acumuladorTiempo += Time.deltaTime;

            while (acumuladorTiempo >= segundosPorMinutoJuego
                   && localAbierto)
            {
                acumuladorTiempo -= segundosPorMinutoJuego;
                minutoActual++;

                if (minutoActual >= 60)
                {
                    minutoActual = 0;
                    horaActual++;

                    if (horaActual >= 24)
                    {
                        horaActual = 0;
                        minutoActual = 0;
                        LlegadaMedianoche();
                    }
                }
            }

            ActualizarInterfazReloj();
        }

        if (esperandoCierre &&
            Input.GetKeyDown(KeyCode.G))
        {
            TerminarDia();
        }
    }

    void AbrirLocal()
    {
        ReiniciarEstadisticasDiarias();

        localAbierto = true;
        esperandoCierre = false;
        horaActual = 11;
        minutoActual = 0;
        acumuladorTiempo = 0f;

        if (textoBalanceUI != null)
            textoBalanceUI.text = "";

        ActualizarInterfazReloj();

        Debug.Log("Comienza el día " + diaActual);
    }

    void LlegadaMedianoche()
    {
        localAbierto = false;
        esperandoCierre = true;

        if (textoRelojUnico != null)
            textoRelojUnico.text =
                "Presione G para acabar con el día";

        Debug.Log("El local cerró. Ya no se registran estadísticas.");
    }


    public void RegistrarClienteAtendido()
    {
        if (!localAbierto) return;

        clientesAtendidosHoy++;
    }

    public void RegistrarVenta(float dinero, bool esCelebridad = false)
    {
        if (!localAbierto) return;

        dineroHoy += dinero;
        shawarmasHoy++;

        if (esCelebridad)
            celebridadesHoy++;
    }

    public void RegistrarGasto(float dinero)
    {
        if (!localAbierto || dinero <= 0) return;

        dineroGastadoHoy += dinero;
    }

    public void RegistrarCambioXP(int cantidad)
    {
        if (!localAbierto) return;

        if (cantidad > 0)
            xpGanadaHoy += cantidad;
        else if (cantidad < 0)
            xpPerdidaHoy -= cantidad;
    }


    void TerminarDia()
    {
        esperandoCierre = false;

        DiaStats estadisticasDia = new DiaStats(
            diaActual,
            clientesAtendidosHoy,
            shawarmasHoy,
            celebridadesHoy,
            dineroHoy,
            dineroGastadoHoy,
            xpGanadaHoy,
            xpPerdidaHoy
        );

        historialDiasAVL.Insert(estadisticasDia);

        string reporteDia = CrearReporteDia(estadisticasDia);

        Debug.Log(reporteDia);

        if (diaActual >= 7)
        {
            string reporteSemanal = GenerarResumenSemanal();

            Debug.Log(reporteSemanal);

            if (textoBalanceUI != null)
                textoBalanceUI.text =
                    reporteDia + "\n\n" + reporteSemanal;

            historialDiasAVL.Clear();
            diaActual = 1;
        }
        else
        {
            if (textoBalanceUI != null)
                textoBalanceUI.text = reporteDia;

            diaActual++;
        }
        if (LogrosManager.Instancia != null)
        {
            LogrosManager.Instancia.DesbloquearLogro("PrimeraRadio");
        }
        ReiniciarEstadisticasDiarias();

        horaActual = 11;
        minutoActual = 0;
        acumuladorTiempo = 0f;

        if (textoRelojUnico != null)
            textoRelojUnico.text = "El día aún no comienza";

        AbrirPanelEstadisticas();
    }

    string CrearReporteDia(DiaStats dia)
    {
        return
            $"=== CIERRE DÍA {dia.numeroDia} ===\n" +
            $"Clientes atendidos: {dia.clientesAtendidos}\n" +
            $"Pedidos correctos: {dia.shawarmasVendidos}\n" +
            $"Pedidos incorrectos: {dia.PedidosIncorrectos}\n" +
            $"Ingresos: ${dia.dineroGanado:0.##}\n" +
            $"Gastos: ${dia.dineroGastado:0.##}\n" +
            $"Balance: ${dia.BalanceNeto:0.##}\n" +
            $"XP ganada: {dia.xpGanada}\n" +
            $"XP perdida: {dia.xpPerdida}\n" +
            $"Balance XP: {dia.BalanceXP}";
    }

    string GenerarResumenSemanal()
    {
        var dias = historialDiasAVL.ObtenerValoresEnOrden();

        int totalAtendidos = 0;
        int totalCorrectos = 0;
        int totalXPGanada = 0;
        int totalXPPerdida = 0;

        float totalIngresos = 0f;
        float totalGastos = 0f;

        StringBuilder reporte = new StringBuilder();

        reporte.AppendLine("=== BALANCE SEMANAL ===");

        foreach (DiaStats dia in dias)
        {
            reporte.AppendLine(
                $"Día {dia.numeroDia}: " +
                $"{dia.clientesAtendidos} atendidos, " +
                $"{dia.shawarmasVendidos} correctos, " +
                $"balance ${dia.BalanceNeto:0.##}"
            );

            totalAtendidos += dia.clientesAtendidos;
            totalCorrectos += dia.shawarmasVendidos;
            totalIngresos += dia.dineroGanado;
            totalGastos += dia.dineroGastado;

            totalXPGanada += dia.xpGanada;
            totalXPPerdida += dia.xpPerdida;
        }

        reporte.AppendLine("--- TOTALES ---");
        reporte.AppendLine($"Clientes: {totalAtendidos}");
        reporte.AppendLine($"Correctos: {totalCorrectos}");
        reporte.AppendLine(
            $"Incorrectos: {totalAtendidos - totalCorrectos}"
        );
        reporte.AppendLine($"Ingresos: ${totalIngresos:0.##}");
        reporte.AppendLine($"Gastos: ${totalGastos:0.##}");
        reporte.AppendLine(
            $"Balance neto: ${totalIngresos - totalGastos:0.##}"
        );
        reporte.AppendLine($"XP ganada: {totalXPGanada}");
        reporte.AppendLine($"XP perdida: {totalXPPerdida}");
        reporte.AppendLine(
            $"Balance XP: {totalXPGanada - totalXPPerdida}"
        );

        return reporte.ToString();
    }

    void ReiniciarEstadisticasDiarias()
    {
        clientesAtendidosHoy = 0;
        shawarmasHoy = 0;
        celebridadesHoy = 0;

        dineroHoy = 0f;
        dineroGastadoHoy = 0f;

        xpGanadaHoy = 0;
        xpPerdidaHoy = 0;
    }

    void ActualizarInterfazReloj()
    {
        if (textoRelojUnico != null && localAbierto)
        {
            textoRelojUnico.text =
                $"{horaActual:00}:{minutoActual:00} (Día {diaActual})";
        }
    }


    public void MostrarEstadisticasEnPausa()
    {
        if (textoBalanceUI == null) return;

        int incorrectos = Mathf.Max(
            0, clientesAtendidosHoy - shawarmasHoy
        );

        textoBalanceUI.text =
            $"=== DIA {diaActual} ===\n\n" +
            $"Clientes atendidos: {clientesAtendidosHoy}\n" +
            $"Pedidos correctos: {shawarmasHoy}\n" +
            $"Pedidos incorrectos: {incorrectos}\n\n" +
            $"Dinero ganado: ${dineroHoy}\n" +
            $"Dinero gastado: ${dineroGastadoHoy}\n" +
            $"Balance: ${dineroHoy - dineroGastadoHoy}\n\n" +
            $"XP ganada: {xpGanadaHoy}\n" +
            $"XP perdida: {xpPerdidaHoy}";

        AbrirPanelEstadisticas();
    }

    public void AbrirPanelEstadisticas()
    {
        if (panelEstadisticas == null) return;

        panelEstadisticas.SetActive(true);
        panelAbierto = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarPanelEstadisticas()
    {
        Time.timeScale = 1f;

        if (panelEstadisticas != null)
            panelEstadisticas.SetActive(false);

        panelAbierto = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}
