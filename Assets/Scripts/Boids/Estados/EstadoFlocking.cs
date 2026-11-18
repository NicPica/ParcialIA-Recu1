public class EstadoFlocking : Estado<EstadoBoid>
{
    private readonly Boid boid;

    public EstadoFlocking(Boid boid)
    {
        this.boid = boid;
    }

    public override void Entrar() { }

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

        if (boid.VeObjeto)
        {
            fsm.CambiarEstado(EstadoBoid.SeekObject);
            return;
        }

        boid.AgregarFuerza(boid.FuerzaSeparacion());
        boid.AgregarFuerza(boid.FuerzaAlineacion());
        boid.AgregarFuerza(boid.FuerzaCohesion());
        boid.AgregarFuerza(boid.FuerzaWander());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());
        boid.AgregarFuerza(boid.FuerzaLimites());
    }

    public override void Salir() { }
}
