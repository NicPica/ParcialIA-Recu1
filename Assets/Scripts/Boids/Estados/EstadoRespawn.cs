using UnityEngine;

public class EstadoRespawn : IEstado
{
    private readonly Boid boid;
    private float tiempo;

    public bool Terminado { get; private set; }

    public EstadoRespawn(Boid boid)
    {
        this.boid = boid;
    }

    public void Entrar()
    {
        tiempo = 0f;
        Terminado = false;
        boid.Ocultar();
    }

    public void Ejecutar()
    {
        if (Terminado) return;

        tiempo += Time.deltaTime;
        if (tiempo >= boid.TiempoRespawn)
        {
            boid.Reaparecer();
            Terminado = true;
        }
    }

    public void Salir() { }
}
