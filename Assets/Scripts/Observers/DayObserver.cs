using UnityEngine;

public class DayObserver
{
    public void Suscribir(Cliente c)
    {
        c.EntregaExitosa += Registrar;
        c.EntregaFallida += Registrar;
    }

    public void Desuscribir(Cliente c)
    {
        c.EntregaExitosa -= Registrar;
        c.EntregaFallida -= Registrar;
    }

    void Registrar(Cliente cliente)
    {
        if (DayManager.Instancia != null)
            DayManager.Instancia.RegistrarClienteAtendido();
    }
}
