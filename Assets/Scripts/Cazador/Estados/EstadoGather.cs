using UnityEngine;

public class EstadoGather : IEstado
{
    private readonly Cazador cazador;
    private Boid objetivo;
    private float tiempo;

    public bool Terminado { get; private set; }

    public EstadoGather(Cazador cazador)
    {
        this.cazador = cazador;
    }

    public void Entrar()
    {
        cazador.CambiarLimiteVelocidad(cazador.VelocidadMax);
        objetivo = cazador.Percepcion.MuertoMasCercano();
        cazador.BoidARecolectar = objetivo;
        tiempo = 0f;
        Terminado = objetivo == null;
    }

    public void Ejecutar()
    {
        if (Terminado) return;

        if (!ObjetivoDisponible())
        {
            Terminado = true;
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
            Terminado = true;
        }
    }

    public void Salir()
    {
        cazador.BoidARecolectar = null;
        objetivo = null;
    }

    private bool ObjetivoDisponible()
    {
        return objetivo != null && objetivo.EstaMuerto && !objetivo.FueRecolectado;
    }
}
