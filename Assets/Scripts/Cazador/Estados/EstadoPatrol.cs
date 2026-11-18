using UnityEngine;

public class EstadoPatrol : Estado<EstadoCazador>
{
    private readonly Cazador cazador;
    private int indice;
    private int sentido = 1;
    private float tiempoObjetos;

    public EstadoPatrol(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public override void Entrar()
    {
        cazador.CambiarLimiteVelocidad(cazador.VelocidadPatrol);
    }

    public override void Ejecutar()
    {
        if (cazador.HayBoidMuerto())
        {
            fsm.CambiarEstado(EstadoCazador.Gather);
            return;
        }

        if (cazador.PuedeAtacar && cazador.Percepcion.VivoMasCercano() != null)
        {
            fsm.CambiarEstado(EstadoCazador.Hunt);
            return;
        }

        if (cazador.Waypoints.Count > 0)
        {
            Vector3 destino = cazador.Waypoints[indice];
            cazador.AgregarFuerza(cazador.FuerzaSeek(destino));

            if (cazador.DistanciaPlana(destino) <= cazador.DistanciaWaypoint)
                AvanzarWaypoint();
        }

        tiempoObjetos += Time.deltaTime;
        if (tiempoObjetos >= cazador.IntervaloObjetos)
        {
            tiempoObjetos = 0f;
            cazador.GenerarObjeto();
        }
    }

    public override void Salir() { }

    private void AvanzarWaypoint()
    {
        int cantidad = cazador.Waypoints.Count;
        if (cantidad < 2) return;

        if (!cazador.IdaYVuelta)
        {
            indice = (indice + 1) % cantidad;
            return;
        }

        if (indice + sentido >= cantidad || indice + sentido < 0)
            sentido = -sentido;
        indice += sentido;
    }
}
