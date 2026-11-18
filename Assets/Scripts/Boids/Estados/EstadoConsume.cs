using UnityEngine;

public class EstadoConsume : Estado<EstadoBoid>
{
    private readonly Boid boid;
    private float tiempo;

    public EstadoConsume(Boid boid)
    {
        this.boid = boid;
    }

    public override void Entrar()
    {
        tiempo = 0f;
    }

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

        ObjetoInteres objeto = boid.ObjetivoActual;
        if (objeto == null)
        {
            fsm.CambiarEstado(EstadoBoid.Flocking);
            return;
        }

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

    public override void Salir()
    {
        boid.ObjetivoActual = null;
    }
}
