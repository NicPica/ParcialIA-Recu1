public class EstadoDead : IEstado
{
    private readonly Boid boid;

    public EstadoDead(Boid boid)
    {
        this.boid = boid;
    }

    public void Entrar()
    {
        boid.Detener();
    }

    public void Ejecutar() { }

    public void Salir() { }
}
