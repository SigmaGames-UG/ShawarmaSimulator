using UnityEngine;

public class HandObserver
{
    public void Suscribir(Cliente c)
    {
        c.EntregaExitosa += _ => Limpiar();
        c.EntregaFallida += _ => Limpiar();
    }

    void Limpiar() => ManagerExperiencia.ingredientesShawarmaEnMano.Clear();
}
