using UnityEngine;

public class EstadoRespawn : Estado<EstadoBoid>
{
    private readonly Boid boid;
    private float tiempo;

    public EstadoRespawn(Boid boid)
    {
        this.boid = boid;
    }

    public override void Entrar()
    {
        tiempo = 0f;
        boid.Ocultar();
    }

    public override void Ejecutar()
    {
        tiempo += Time.deltaTime;
        if (tiempo >= boid.TiempoRespawn)
        {
            boid.Reaparecer();
            fsm.CambiarEstado(EstadoBoid.Flocking);
        }
    }

    public override void Salir() { }
}
