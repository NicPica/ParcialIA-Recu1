public class EstadoRest : IEstado
{
    private readonly Cazador cazador;

    public EstadoRest(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public void Entrar()
    {
        cazador.Detener();
    }

    public void Ejecutar()
    {
        cazador.Detener();
        cazador.RecargarEnergia();
    }

    public void Salir() { }
}
