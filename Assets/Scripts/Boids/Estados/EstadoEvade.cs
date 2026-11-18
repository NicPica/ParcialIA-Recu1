public class EstadoEvade : Estado<EstadoBoid>
{
    private readonly Boid boid;

    public EstadoEvade(Boid boid)
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

        if (!boid.VeCazador)
        {
            fsm.CambiarEstado(boid.VeObjeto ? EstadoBoid.SeekObject : EstadoBoid.Flocking);
            return;
        }

        boid.AgregarFuerza(boid.FuerzaEvade(boid.Percepcion.Cazador));
        boid.AgregarFuerza(boid.FuerzaSeparacionEvade());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());
        boid.AgregarFuerza(boid.FuerzaLimites());
    }

    public override void Salir() { }
}
