using UnityEngine;

public class EstadoConsume : IEstado
{
    private readonly Boid boid;
    private float tiempo;

    public EstadoConsume(Boid boid)
    {
        this.boid = boid;
    }

    public void Entrar()
    {
        tiempo = 0f;
    }

    public void Ejecutar()
    {
        ObjetoInteres objeto = boid.ObjetivoActual;
        if (objeto == null) return;

        boid.AgregarFuerza(boid.FuerzaArrive(objeto));
        boid.AgregarFuerza(boid.FuerzaSeparacion());
        boid.AgregarFuerza(boid.FuerzaEvitarObjetos());

        tiempo += Time.deltaTime;
        if (tiempo >= boid.IntervaloConsumo)
        {
            tiempo = 0f;
            objeto.RecibirDanio(boid.DanioPorConsumo);
        }
    }

    public void Salir()
    {
        boid.ObjetivoActual = null;
    }
}
