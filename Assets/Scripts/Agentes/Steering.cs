using UnityEngine;

public static class Steering
{
    public static Vector3 HaciaDireccion(Agente agente, Vector3 direccion)
    {
        direccion.y = 0f;
        if (direccion.sqrMagnitude < 0.0001f) return Vector3.zero;
        return direccion.normalized * agente.LimiteVelocidad - agente.Velocidad;
    }

    public static Vector3 Seek(Agente agente, Vector3 objetivo)
    {
        return HaciaDireccion(agente, objetivo - agente.transform.position);
    }

    public static Vector3 Flee(Agente agente, Vector3 amenaza)
    {
        return HaciaDireccion(agente, agente.transform.position - amenaza);
    }

    public static Vector3 Arrive(Agente agente, Vector3 objetivo, float radioFrenado, float distanciaParada)
    {
        Vector3 hacia = objetivo - agente.transform.position;
        hacia.y = 0f;

        float distancia = hacia.magnitude - distanciaParada;
        if (distancia <= 0f) return -agente.Velocidad;

        float rapidez = agente.LimiteVelocidad;
        if (distancia < radioFrenado)
            rapidez *= distancia / radioFrenado;

        return hacia.normalized * rapidez - agente.Velocidad;
    }

    public static Vector3 Pursuit(Agente agente, Agente objetivo, float prediccionMax)
    {
        return Seek(agente, PosicionFutura(agente, objetivo, prediccionMax));
    }

    public static Vector3 Evade(Agente agente, Agente amenaza, float prediccionMax)
    {
        return Flee(agente, PosicionFutura(agente, amenaza, prediccionMax));
    }

    public static Vector3 Wander(Agente agente, ref float angulo, float distancia, float radio, float variacion)
    {
        angulo += Random.Range(-variacion, variacion) * Time.deltaTime;

        Vector3 centro = agente.transform.position + agente.transform.forward * distancia;
        float radianes = angulo * Mathf.Deg2Rad;
        Vector3 desplazamiento = new Vector3(Mathf.Cos(radianes), 0f, Mathf.Sin(radianes)) * radio;

        return Seek(agente, centro + desplazamiento);
    }

    private static Vector3 PosicionFutura(Agente agente, Agente objetivo, float prediccionMax)
    {
        float distancia = Vector3.Distance(agente.transform.position, objetivo.transform.position);
        float tiempo = Mathf.Min(distancia / Mathf.Max(agente.LimiteVelocidad, 0.01f), prediccionMax);
        return objetivo.transform.position + objetivo.Velocidad * tiempo;
    }
}
