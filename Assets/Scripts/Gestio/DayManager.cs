using UnityEngine;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static DayManager Instancia;

    [Header("UI del Reloj y Avisos")]
    public TextMeshProUGUI textoRelojUnico; // Un solo componente de texto que cambia de estado

    [Header("Configuración de Tiempo")]
    public int horaActual = 11;
    public int minutoActual = 0;
    private bool localAbierto = false;
    private bool esperandoCierre = false;
    private float acumuladorTiempo = 0f;

    // 1 minuto real = 1 hora in-game -> 1 segundo real = 1 minuto in-game
    private const float segundosPorMinutoJuego = 1f;

    [Header("Estadísticas del Día Actual")]
    public int diaActual = 1;
    public float dineroHoy = 0f;
    public int shawarmasHoy = 0;
    public int celebridadesHoy = 0;

    // Árbol AVL para el sistema de DataDay y resumen semanal
    private AVL<DiaStats> historialDiasAVL = new AVL<DiaStats>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Al arrancar, mostramos el mensaje inicial requerido
        if (textoRelojUnico != null)
        {
            textoRelojUnico.text = "El día aún no comienza";
        }
    }

    void Update()
    {
        // 1. Abrir el local con la tecla G (cuando el día aún no arranca)
        if (!localAbierto && !esperandoCierre && Input.GetKeyDown(KeyCode.G))
        {
            AbrirLocal();
        }

        // 2. Si el local está abierto, corre el tiempo del temporizador
        if (localAbierto)
        {
            acumuladorTiempo += Time.deltaTime;
            if (acumuladorTiempo >= segundosPorMinutoJuego)
            {
                acumuladorTiempo = 0f;
                minutoActual++;

                if (minutoActual >= 60)
                {
                    minutoActual = 0;
                    horaActual++;

                    // A las 00:00 (12 de la noche) se detiene el tiempo y cambia el cartel
                    if (horaActual >= 24)
                    {
                        horaActual = 0;
                        minutoActual = 0;
                        LlegadaMedianoche();
                    }
                }
                ActualizarInterfazReloj();
            }
        }

        // 3. Terminar el día presionando G cuando llega a las 00:00
        if (esperandoCierre && Input.GetKeyDown(KeyCode.G))
        {
            TerminarDia();
        }
    }

    void AbrirLocal()
    {
        localAbierto = true;
        horaActual = 11;
        minutoActual = 0;
        ActualizarInterfazReloj();
        Debug.Log("¡Local abierto! Comienza el día " + diaActual);
    }

    void LlegadaMedianoche()
    {
        localAbierto = false;
        esperandoCierre = true;

        // Cambiamos el texto al mensaje de cierre requerido
        if (textoRelojUnico != null)
        {
            textoRelojUnico.text = "Presione G para acabar con el día";
        }
        Debug.Log("Medianoche alcanzada. Pulsa G para finalizar el día.");
    }

    void TerminarDia()
    {
        esperandoCierre = false;

        // Guardamos los datos del día en el Árbol AVL
        DiaStats estadisticasDia = new DiaStats(diaActual, dineroHoy, shawarmasHoy, celebridadesHoy);
        historialDiasAVL.Insert(estadisticasDia);

        Debug.Log($"--- RESUMEN DÍA {diaActual} GUARDADO EN AVL ---");
        Debug.Log($"Dinero: ${dineroHoy} | Shawarmas: {shawarmasHoy} | Celebridades: {celebridadesHoy}");

        // Si es el séptimo día, mostramos el resumen semanal
        if (diaActual >= 7)
        {
            GenerarResumenSemanal();
        }
        else
        {
            // Preparamos el siguiente día
            diaActual++;
            horaActual = 11;
            minutoActual = 0;
            dineroHoy = 0f;
            shawarmasHoy = 0;
            celebridadesHoy = 0;

            // Volvemos a poner el estado inicial para el nuevo día
            if (textoRelojUnico != null)
            {
                textoRelojUnico.text = "El día aún no comienza";
            }
        }
    }

    void GenerarResumenSemanal()
    {
        Debug.Log("=== ¡FIN DE LA SEMANA! BALANCE GENERAL ===");

        // Reiniciamos el ciclo semanal
        diaActual = 1;
        horaActual = 11;
        minutoActual = 0;
        dineroHoy = 0f;
        shawarmasHoy = 0;
        celebridadesHoy = 0;

        if (textoRelojUnico != null)
        {
            textoRelojUnico.text = "El día aún no comienza";
        }
    }

    void ActualizarInterfazReloj()
    {
        if (textoRelojUnico != null && localAbierto)
        {
            // Muestra el formato de hora HH:mm y el día actual
            textoRelojUnico.text = $"{horaActual:00}:{minutoActual:00} (Día {diaActual})";
        }
    }

    // Método para registrar ventas durante el juego
    public void RegistrarVenta(float dinero, bool esCelebridad = false)
    {
        if (!localAbierto) return;
        dineroHoy += dinero;
        shawarmasHoy++;
        if (esCelebridad) celebridadesHoy++;
    }
}