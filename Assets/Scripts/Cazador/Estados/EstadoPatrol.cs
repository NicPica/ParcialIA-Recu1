using UnityEngine;

public class EstadoPatrol : IEstado
{
    private readonly Cazador cazador;
    private int indice;
    private int sentido = 1;
    private float tiempoObjetos;

    public EstadoPatrol(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public void Entrar()
    {
        cazador.CambiarLimiteVelocidad(cazador.VelocidadPatrol);
    }

    public void Ejecutar()
    {
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

    public void Salir() { }

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
