using System.Collections.Generic;
using UnityEngine;

public enum EstadoCazador { Patrol, Hunt, Gather, Rest }

[RequireComponent(typeof(PercepcionCazador))]
public class Cazador : Agente
{
    [Header("Velocidades")]
    [SerializeField] private float velocidadPatrol = 3f;
    [SerializeField] private float radioFrenado = 2f;

    [Header("Ataque")]
    [SerializeField] private float tba = 1.5f;
    [SerializeField] private float rangeAttackRadius = 4f;
    [SerializeField] private float meleeAttackRadius = 1.5f;
    [SerializeField] private int golpesMelee = 2;
    [SerializeField] private int golpesRango = 1;
    [SerializeField] private float prediccionMax = 1f;
    [SerializeField] private float distanciaAcercamiento = 0.75f;
    [SerializeField] private float distanciaParadaUltimaPosicion = 0.2f;

    [Header("Percepcion")]
    [SerializeField] private float visionRadius = 9f;

    [Header("Energia")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float consumoEnergia = 8f;
    [SerializeField] private float recargaEnergia = 20f;

    [Header("Patrol")]
    [SerializeField] private List<Vector3> waypoints = new List<Vector3>();
    [SerializeField] private float distanciaWaypointsAlBorde = 6f;
    [SerializeField] private float distanciaWaypoint = 1f;
    [SerializeField] private bool idaYVuelta;

    [Header("Objetos de interes")]
    [SerializeField] private ObjetoInteres prefabObjeto;
    [SerializeField] private float intervaloObjetos = 4f;
    [SerializeField] private int maxObjetos = 5;
    [SerializeField] private float espacioLibreObjeto = 1.5f;
    [SerializeField] private float distanciaObjetosAlBorde = 3f;
    [SerializeField] private LayerMask capasOcupadas;

    [Header("Gather")]
    [SerializeField] private float distanciaRecoleccion = 1.3f;
    [SerializeField] private float tiempoRecoleccion = 2f;

    [Header("Gizmos")]
    [SerializeField] private bool mostrarGizmos = true;

    private MaquinaEstados<EstadoCazador> fsm;
    private float tiempoParaAtacar;
    private float energy;

    public PercepcionCazador Percepcion { get; private set; }
    public Boid Presa { get; set; }
    public Boid BoidARecolectar { get; set; }
    public EstadoCazador EstadoActual => fsm.Actual;

    public bool PuedeAtacar => tiempoParaAtacar <= 0f;
    public float Energy => energy;
    public float MaxEnergy => maxEnergy;
    public float VisionRadius => visionRadius;
    public float MeleeAttackRadius => meleeAttackRadius;
    public float RangeAttackRadius => rangeAttackRadius;
    public float VelocidadPatrol => velocidadPatrol;
    public IReadOnlyList<Vector3> Waypoints => waypoints;
    public float DistanciaWaypoint => distanciaWaypoint;
    public bool IdaYVuelta => idaYVuelta;
    public float IntervaloObjetos => intervaloObjetos;
    public float DistanciaRecoleccion => distanciaRecoleccion;
    public float TiempoRecoleccion => tiempoRecoleccion;
    public float DistanciaAcercamiento => distanciaAcercamiento;
    public float DistanciaParadaUltimaPosicion => distanciaParadaUltimaPosicion;

    public event System.Action<EstadoCazador> OnCambioEstado;
    public event System.Action<Boid, bool> OnAtaque;

    protected override void Awake()
    {
        base.Awake();
        Percepcion = GetComponent<PercepcionCazador>();
        energy = maxEnergy;
        CrearFSM();
    }

    private void Start()
    {
        if (waypoints.Count == 0 && Limites.Instancia != null)
            waypoints.AddRange(Limites.Instancia.Esquinas(distanciaWaypointsAlBorde));

        fsm.Iniciar(EstadoCazador.Patrol);
    }

    private void Update()
    {
        tiempoParaAtacar = Mathf.Max(0f, tiempoParaAtacar - Time.deltaTime);
        fsm.Actualizar();
        AplicarMovimiento();
    }

    private void CrearFSM()
    {
        fsm = new MaquinaEstados<EstadoCazador>();
        fsm.OnCambioEstado += estado => OnCambioEstado?.Invoke(estado);

        EstadoGather gather = new EstadoGather(this);

        fsm.AgregarEstado(EstadoCazador.Patrol, new EstadoPatrol(this));
        fsm.AgregarEstado(EstadoCazador.Hunt, new EstadoHunt(this));
        fsm.AgregarEstado(EstadoCazador.Gather, gather);
        fsm.AgregarEstado(EstadoCazador.Rest, new EstadoRest(this));

        fsm.AgregarTransicion(EstadoCazador.Patrol, EstadoCazador.Gather, HayBoidMuerto);
        fsm.AgregarTransicion(EstadoCazador.Patrol, EstadoCazador.Hunt, () => PuedeAtacar && Percepcion.VivoMasCercano() != null);

        fsm.AgregarTransicion(EstadoCazador.Hunt, EstadoCazador.Rest, () => energy <= 0f);
        fsm.AgregarTransicion(EstadoCazador.Hunt, EstadoCazador.Patrol, PresaFueraDeLimites);
        fsm.AgregarTransicion(EstadoCazador.Hunt, EstadoCazador.Gather, HayBoidMuerto);

        fsm.AgregarTransicion(EstadoCazador.Gather, EstadoCazador.Patrol, () => gather.Terminado);

        fsm.AgregarTransicion(EstadoCazador.Rest, EstadoCazador.Patrol, () => energy >= maxEnergy);
    }

    private bool HayBoidMuerto()
    {
        return Percepcion.MuertoMasCercano() != null;
    }

    private bool PresaFueraDeLimites()
    {
        return Presa != null && Limites.Instancia != null && !Limites.Instancia.Contiene(Presa.transform.position);
    }

    public void Atacar(Boid presa, bool cuerpoACuerpo)
    {
        presa.RecibirGolpes(cuerpoACuerpo ? golpesMelee : golpesRango);
        tiempoParaAtacar = tba;
        OnAtaque?.Invoke(presa, cuerpoACuerpo);
    }

    public void ConsumirEnergia()
    {
        energy = Mathf.Max(0f, energy - consumoEnergia * Time.deltaTime);
    }

    public void RecargarEnergia()
    {
        energy = Mathf.Min(maxEnergy, energy + recargaEnergia * Time.deltaTime);
    }

    public void GenerarObjeto()
    {
        if (prefabObjeto == null || Limites.Instancia == null) return;
        if (ObjetoInteres.Activos >= maxObjetos) return;

        Vector3 posicion = Limites.Instancia.PosicionLibre(distanciaObjetosAlBorde, espacioLibreObjeto, capasOcupadas);
        Instantiate(prefabObjeto, posicion, Quaternion.identity);
    }

    public Vector3 FuerzaSeek(Vector3 punto)
    {
        return Steering.Seek(this, punto);
    }

    public Vector3 FuerzaArrive(Vector3 punto, float distanciaParada)
    {
        return Steering.Arrive(this, punto, radioFrenado, distanciaParada);
    }

    public Vector3 FuerzaPursuit(Agente presa)
    {
        return Steering.Pursuit(this, presa, prediccionMax);
    }

    private void OnValidate()
    {
        meleeAttackRadius = Mathf.Clamp(meleeAttackRadius, 0.1f, rangeAttackRadius - 0.1f);
        distanciaAcercamiento = Mathf.Clamp(distanciaAcercamiento, 0f, meleeAttackRadius);
        maxObjetos = Mathf.Max(1, maxObjetos);
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        Vector3 posicion = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(posicion, visionRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(posicion, rangeAttackRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(posicion, meleeAttackRadius);

        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Gizmos.DrawSphere(waypoints[i], 0.3f);
            if (i > 0) Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
        }

        if (!Application.isPlaying || Percepcion == null) return;

        Gizmos.color = Color.green;
        foreach (Boid boid in Percepcion.Vivos)
            if (boid != null) Gizmos.DrawLine(posicion, boid.transform.position);

        Gizmos.color = Color.gray;
        foreach (Boid boid in Percepcion.Muertos)
            if (boid != null) Gizmos.DrawLine(posicion, boid.transform.position);

        if (Presa != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(posicion + Vector3.up, Presa.transform.position + Vector3.up);
        }
    }
}
