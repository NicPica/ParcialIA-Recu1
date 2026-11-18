using UnityEngine;

public class EstadoHunt : Estado<EstadoCazador>
{
    private readonly Cazador cazador;
    private Vector3 ultimaPosicion;

    public EstadoHunt(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public override void Entrar()
    {
        cazador.CambiarLimiteVelocidad(cazador.VelocidadMax);
        cazador.Presa = cazador.Percepcion.VivoMasCercano();
        ultimaPosicion = cazador.Presa != null ? cazador.Presa.transform.position : cazador.transform.position;
    }

    public override void Ejecutar()
    {
        if (cazador.Energy <= 0f)
        {
            fsm.CambiarEstado(EstadoCazador.Rest);
            return;
        }

        if (cazador.PresaFueraDeLimites())
        {
            fsm.CambiarEstado(EstadoCazador.Patrol);
            return;
        }

        if (cazador.HayBoidMuerto())
        {
            fsm.CambiarEstado(EstadoCazador.Gather);
            return;
        }

        cazador.ConsumirEnergia();

        if (cazador.Presa == null || cazador.Presa.EstaMuerto)
            cazador.Presa = cazador.Percepcion.VivoMasCercano();

        Boid presa = cazador.Presa;
        if (presa == null)
        {
            cazador.AgregarFuerza(cazador.FuerzaArrive(ultimaPosicion, cazador.DistanciaParadaUltimaPosicion));
            return;
        }

        ultimaPosicion = presa.transform.position;
        float distancia = cazador.DistanciaPlana(ultimaPosicion);

        if (cazador.PuedeAtacar)
        {
            if (distancia <= cazador.MeleeAttackRadius)
                cazador.Atacar(presa, true);
            else if (distancia <= cazador.RangeAttackRadius)
                cazador.Atacar(presa, false);
        }

        if (distancia > cazador.DistanciaAcercamiento)
            cazador.AgregarFuerza(cazador.FuerzaPursuit(presa));
        else
            cazador.AgregarFuerza(cazador.FuerzaArrive(ultimaPosicion, cazador.DistanciaAcercamiento));
    }

    public override void Salir()
    {
        cazador.Presa = null;
    }
}
