using UnityEngine;

public class DayObserver
{
    public void Suscribir(Cliente c)
    {
        c.EntregaExitosa += _ => Registrar();
        c.EntregaFallida += _ => Registrar();
    }

    void Registrar()
    {
        if (DayManager.Instancia != null)
            DayManager.Instancia.RegistrarClienteAtendido();
    }
}
