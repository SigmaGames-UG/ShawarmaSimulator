public class TirarShawarmaCommand : ICommand
{
    private ArmadoShawarma plato;

    public TirarShawarmaCommand(ArmadoShawarma plato)
    {
        this.plato = plato;
    }

    public void Ejecutar()
    {
        if (plato != null)
        {
            plato.TirarShawarma();
        }
    }
}