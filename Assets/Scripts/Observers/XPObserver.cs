using UnityEngine;

public class XPObserver
{
    public void Suscribir(Cliente c)
    {
        c.EntregaExitosa += OnExito;
        c.EntregaFallida += OnFallo;
        c.ClienteSeFue += OnSeFue;
    }

    public void Desuscribir(Cliente c)
    {
        c.EntregaExitosa -= OnExito;
        c.EntregaFallida -= OnFallo;
        c.ClienteSeFue -= OnSeFue;
    }

    void OnExito(Cliente c)
    {
        
        if (ManagerExperiencia.Instancia != null)
            ManagerExperiencia.Instancia.RegistrarVentaShawarma(c.CantidadIngredientes);
    }

    void OnFallo(Cliente c)
    {
        
        if (ManagerExperiencia.Instancia != null)
            ManagerExperiencia.Instancia.RegistrarPedidoIncorrecto(10);
    }

    void OnSeFue(Cliente c)
    {
        
        if (ManagerExperiencia.Instancia != null)
            ManagerExperiencia.Instancia.ModificarXP(-15);
    }
}
