public class EstadoSeekObject : Estado<EstadoBoid>
{
    private readonly Boid boid;

    public EstadoSeekObject(Boid boid)
    {
        this.boid = boid;
    }

    public override void Entrar()
    {
        boid.ObjetivoActual = boid.Percepcion.ObjetoMasCercano;
    }

    public override void Ejecutar()
    {
        if (boid.EstaMuerto)
        {
            fsm.CambiarEstado(EstadoBoid.Dead);
            return;
        }

        if (boid.VeCazador)
        {
            fsm.CambiarEstado(EstadoBoid.Evade);
            return;
        }

        if (boid.LlegoAlObjetivo())
        {
            fsm.CambiarEstado(EstadoBoid.Consume);
            return;
        }

        if (boid.ObjetivoActual == null)
        {
            fsm.CambiarEstado(EstadoBoid.Flocking);
            return;
        }

        boid.AgregarFuerza(boid.FuerzaArrive(boid.ObjetivoActual));
        boid.AgregarFuerza(boid.FuerzaSeparacion());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());
        boid.AgregarFuerza(boid.FuerzaLimites());
    }

    public override void Salir() { }
}
