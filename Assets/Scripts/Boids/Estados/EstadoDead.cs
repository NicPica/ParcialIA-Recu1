public class EstadoDead : Estado<EstadoBoid>
{
    private readonly Boid boid;

    public EstadoDead(Boid boid)
    {
        this.boid = boid;
    }

    public override void Entrar()
    {
        boid.Detener();
    }

    public override void Ejecutar()
    {
        if (boid.FueRecolectado)
            fsm.CambiarEstado(EstadoBoid.Respawn);
    }

    public override void Salir() { }
}
