using UnityEngine;

public class HandObserver
{
    public void Suscribir(Cliente c)
    {
        c.EntregaExitosa += Limpiar;
        c.EntregaFallida += Limpiar;
    }

    public void Desuscribir(Cliente c)
    {
        c.EntregaExitosa -= Limpiar;
        c.EntregaFallida -= Limpiar;
    }

    void Limpiar(Cliente cliente) => ManagerExperiencia.ingredientesShawarmaEnMano.Clear();
}
