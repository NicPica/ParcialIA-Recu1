public class EstadoSeekObject : IEstado
{
    private readonly Boid boid;

    public EstadoSeekObject(Boid boid)
    {
        this.boid = boid;
    }

    public void Entrar()
    {
        boid.ObjetivoActual = boid.Percepcion.ObjetoMasCercano;
    }

    public void Ejecutar()
    {
        if (boid.ObjetivoActual == null) return;

        boid.AgregarFuerza(boid.FuerzaArrive(boid.ObjetivoActual));
        boid.AgregarFuerza(boid.FuerzaSeparacion());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());
        boid.AgregarFuerza(boid.FuerzaLimites());
    }

    public void Salir() { }
}
