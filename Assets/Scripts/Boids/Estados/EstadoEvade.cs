public class EstadoEvade : IEstado
{
    private readonly Boid boid;

    public EstadoEvade(Boid boid)
    {
        this.boid = boid;
    }

    public void Entrar() { }

    public void Ejecutar()
    {
        Agente cazador = boid.Percepcion.Cazador;
        if (cazador != null)
            boid.AgregarFuerza(boid.FuerzaEvade(cazador));

        boid.AgregarFuerza(boid.FuerzaSeparacionEvade());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());
        boid.AgregarFuerza(boid.FuerzaLimites());
    }

    public void Salir() { }
}
