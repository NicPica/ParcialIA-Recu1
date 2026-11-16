public class EstadoFlocking : IEstado
{
    private readonly Boid boid;

    public EstadoFlocking(Boid boid)
    {
        this.boid = boid;
    }

    public void Entrar() { }

    public void Ejecutar()
    {
        boid.AgregarFuerza(boid.FuerzaSeparacion());
        boid.AgregarFuerza(boid.FuerzaAlineacion());
        boid.AgregarFuerza(boid.FuerzaCohesion());
        boid.AgregarFuerza(boid.FuerzaWander());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());
        boid.AgregarFuerza(boid.FuerzaLimites());
    }

    public void Salir() { }
}
