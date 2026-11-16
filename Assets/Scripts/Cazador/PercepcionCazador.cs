using System.Collections.Generic;
using UnityEngine;

public class PercepcionCazador : MonoBehaviour
{
    [SerializeField] private float intervalo = 0.2f;
    [SerializeField] private int capacidad = 40;
    [SerializeField] private LayerMask capaBoids;

    private Cazador cazador;
    private Collider[] resultados;
    private float tiempoRestante;

    public List<Boid> Vivos { get; } = new List<Boid>();
    public List<Boid> Muertos { get; } = new List<Boid>();

    private void Awake()
    {
        cazador = GetComponent<Cazador>();
        resultados = new Collider[capacidad];
    }

    private void Update()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante > 0f) return;

        tiempoRestante = intervalo;
        Detectar();
    }

    private void Detectar()
    {
        Vivos.Clear();
        Muertos.Clear();

        int cantidad = Physics.OverlapSphereNonAlloc(transform.position, cazador.VisionRadius, resultados, capaBoids, QueryTriggerInteraction.Collide);

        for (int i = 0; i < cantidad; i++)
        {
            if (!resultados[i].TryGetComponent(out Boid boid)) continue;

            if (boid.EstaMuerto) Muertos.Add(boid);
            else Vivos.Add(boid);
        }
    }

    public Boid VivoMasCercano()
    {
        Boid masCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (Boid boid in Vivos)
        {
            if (boid == null || boid.EstaMuerto) continue;

            float distancia = cazador.DistanciaPlana(boid.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                masCercano = boid;
            }
        }

        return masCercano;
    }

    public Boid MuertoMasCercano()
    {
        Boid masCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (Boid boid in Muertos)
        {
            if (boid == null || !boid.EstaMuerto || boid.FueRecolectado) continue;

            float distancia = cazador.DistanciaPlana(boid.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                masCercano = boid;
            }
        }

        return masCercano;
    }
}
