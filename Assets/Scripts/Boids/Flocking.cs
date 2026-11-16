using System.Collections.Generic;
using UnityEngine;

public static class Flocking
{
    public static Vector3 Separacion(Agente agente, List<Boid> vecinos, float radio)
    {
        Vector3 alejarse = Vector3.zero;

        foreach (Boid otro in vecinos)
        {
            if (otro == null) continue;

            Vector3 diferencia = agente.transform.position - otro.transform.position;
            diferencia.y = 0f;
            float distancia = diferencia.magnitude;
            if (distancia > radio || distancia < 0.001f) continue;

            alejarse += diferencia.normalized * (radio - distancia) / radio;
        }

        return Steering.HaciaDireccion(agente, alejarse);
    }

    public static Vector3 Alineacion(Agente agente, List<Boid> vecinos)
    {
        Vector3 suma = Vector3.zero;
        int cantidad = 0;

        foreach (Boid otro in vecinos)
        {
            if (otro == null || otro.EstaMuerto) continue;
            suma += otro.Velocidad;
            cantidad++;
        }

        if (cantidad == 0) return Vector3.zero;
        return Steering.HaciaDireccion(agente, suma / cantidad);
    }

    public static Vector3 Cohesion(Agente agente, List<Boid> vecinos)
    {
        Vector3 centro = Vector3.zero;
        int cantidad = 0;

        foreach (Boid otro in vecinos)
        {
            if (otro == null || otro.EstaMuerto) continue;
            centro += otro.transform.position;
            cantidad++;
        }

        if (cantidad == 0) return Vector3.zero;
        return Steering.Seek(agente, centro / cantidad);
    }
}
