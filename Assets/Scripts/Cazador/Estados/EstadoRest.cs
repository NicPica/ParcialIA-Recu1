public class EstadoRest : Estado<EstadoCazador>
{
    private readonly Cazador cazador;

    public EstadoRest(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public override void Entrar()
    {
        cazador.Detener();
    }

    public override void Ejecutar()
    {
        if (cazador.Energy >= cazador.MaxEnergy)
        {
            fsm.CambiarEstado(EstadoCazador.Patrol);
            return;
        }

        cazador.Detener();
        cazador.RecargarEnergia();
    }

    public override void Salir() { }
}
