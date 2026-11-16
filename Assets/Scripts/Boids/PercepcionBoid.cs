using System.Collections.Generic;
using UnityEngine;

public class PercepcionBoid : MonoBehaviour
{
    [SerializeField] private float intervalo = 0.2f;
    [SerializeField] private int capacidad = 32;
    [SerializeField] private LayerMask capaBoids;
    [SerializeField] private LayerMask capaCazador;
    [SerializeField] private LayerMask capaObjetos;

    private Boid boid;
    private Collider[] resultados;
    private float tiempoRestante;

    public List<Boid> Vecinos { get; } = new List<Boid>();
    public List<ObjetoInteres> Objetos { get; } = new List<ObjetoInteres>();
    public Agente Cazador { get; private set; }
    public ObjetoInteres ObjetoMasCercano { get; private set; }
    public int CapasOcupadas => capaBoids | capaCazador | capaObjetos;

    private void Awake()
    {
        boid = GetComponent<Boid>();
        resultados = new Collider[capacidad];
        tiempoRestante = Random.Range(0f, intervalo);
    }

    private void Update()
    {
        if (boid.EstaMuerto) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante > 0f) return;

        tiempoRestante = intervalo;
        Detectar();
    }

    private void Detectar()
    {
        Vecinos.Clear();
        Objetos.Clear();
        Cazador = null;
        ObjetoMasCercano = null;

        float radio = Mathf.Max(boid.RadioVision, boid.RadioVecinos);
        int mascara = capaBoids | capaCazador | capaObjetos;
        int cantidad = Physics.OverlapSphereNonAlloc(transform.position, radio, resultados, mascara, QueryTriggerInteraction.Collide);

        float menorDistancia = float.MaxValue;

        for (int i = 0; i < cantidad; i++)
        {
            Collider detectado = resultados[i];
            int capa = 1 << detectado.gameObject.layer;
            float distancia = boid.DistanciaPlana(detectado.transform.position);

            if ((capa & capaBoids) != 0)
            {
                if (distancia <= boid.RadioVecinos && detectado.TryGetComponent(out Boid otro) && otro != boid)
                    Vecinos.Add(otro);
            }
            else if ((capa & capaCazador) != 0)
            {
                if (distancia <= boid.RadioVision)
                    Cazador = detectado.GetComponent<Agente>();
            }
            else if ((capa & capaObjetos) != 0)
            {
                if (distancia > boid.RadioVision || !detectado.TryGetComponent(out ObjetoInteres objeto)) continue;

                Objetos.Add(objeto);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    ObjetoMasCercano = objeto;
                }
            }
        }
    }
}
