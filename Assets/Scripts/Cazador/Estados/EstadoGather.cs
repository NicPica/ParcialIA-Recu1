using UnityEngine;

public class EstadoGather : Estado<EstadoCazador>
{
    private readonly Cazador cazador;
    private Boid objetivo;
    private float tiempo;

    public EstadoGather(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public override void Entrar()
    {
        cazador.CambiarLimiteVelocidad(cazador.VelocidadMax);
        objetivo = cazador.Percepcion.MuertoMasCercano();
        cazador.BoidARecolectar = objetivo;
        tiempo = 0f;
    }

    public override void Ejecutar()
    {
        if (!ObjetivoDisponible())
        {
            fsm.CambiarEstado(EstadoCazador.Patrol);
            return;
        }

        Vector3 posicion = objetivo.transform.position;
        if (cazador.DistanciaPlana(posicion) > cazador.DistanciaRecoleccion)
        {
            cazador.AgregarFuerza(cazador.FuerzaSeek(posicion));
            return;
        }

        cazador.Detener();
        tiempo += Time.deltaTime;
        if (tiempo >= cazador.TiempoRecoleccion)
        {
            objetivo.Recolectar();
            fsm.CambiarEstado(EstadoCazador.Patrol);
        }
    }

    public override void Salir()
    {
        cazador.BoidARecolectar = null;
        objetivo = null;
    }

    private bool ObjetivoDisponible()
    {
        return objetivo != null && objetivo.EstaMuerto && !objetivo.FueRecolectado;
    }
}
