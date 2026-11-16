using UnityEngine;

public enum EstadoBoid { Flocking, Evade, SeekObject, Consume, Dead, Respawn }

[RequireComponent(typeof(PercepcionBoid))]
public class Boid : Agente
{
    [Header("Velocidad")]
    [SerializeField] private float velocidadInicial = 2f;

    [Header("Vida")]
    [SerializeField] private int vidaMax = 2;

    [Header("Cuerpo")]
    [SerializeField] private float radioCuerpo = 0.4f;

    [Header("Percepcion")]
    [SerializeField] private float radioVision = 6f;

    [Header("Flocking")]
    [SerializeField] private float radioSeparacion = 1.5f;
    [SerializeField] private float radioVecinos = 5f;
    [SerializeField] private float pesoSeparacion = 2.5f;
    [SerializeField] private float pesoAlineacion = 1f;
    [SerializeField] private float pesoCohesion = 1f;
    [SerializeField] private float pesoWander = 0.4f;

    [Header("Wander")]
    [SerializeField] private float distanciaWander = 2f;
    [SerializeField] private float radioWander = 1f;
    [SerializeField] private float variacionWander = 180f;

    [Header("Evade")]
    [SerializeField] private float pesoEvade = 3f;
    [SerializeField] private float pesoSeparacionEvade = 3f;
    [SerializeField] private float prediccionMax = 1f;

    [Header("Objetos de interes")]
    [SerializeField] private float pesoArrive = 2f;
    [SerializeField] private float radioFrenado = 3f;
    [SerializeField] private float margenObjeto = 0.4f;
    [SerializeField] private float toleranciaLlegada = 0.3f;
    [SerializeField] private float pesoEvitarObjetos = 3f;
    [SerializeField] private float intervaloConsumo = 0.5f;
    [SerializeField] private int danioPorConsumo = 1;

    [Header("Respawn")]
    [SerializeField] private float tiempoRespawn = 3f;
    [SerializeField] private float distanciaRespawnAlBorde = 2f;

    [Header("Limites")]
    [SerializeField] private float pesoLimites = 5f;

    [Header("Gizmos")]
    [SerializeField] private bool mostrarGizmos = true;

    private MaquinaEstados<EstadoBoid> fsm;
    private Renderer[] renderers;
    private Collider colisionador;
    private int vida;
    private float anguloWander;

    public PercepcionBoid Percepcion { get; private set; }
    public ObjetoInteres ObjetivoActual { get; set; }
    public bool FueRecolectado { get; private set; }
    public bool EstaMuerto => vida <= 0;
    public int Vida => vida;
    public int VidaMax => vidaMax;
    public float RadioVision => radioVision;
    public float RadioVecinos => radioVecinos;
    public float IntervaloConsumo => intervaloConsumo;
    public int DanioPorConsumo => danioPorConsumo;
    public float TiempoRespawn => tiempoRespawn;
    public EstadoBoid EstadoActual => fsm.Actual;

    public event System.Action<EstadoBoid> OnCambioEstado;
    public event System.Action OnGolpeado;

    protected override void Awake()
    {
        base.Awake();
        Percepcion = GetComponent<PercepcionBoid>();
        renderers = GetComponentsInChildren<Renderer>();
        colisionador = GetComponent<Collider>();
        vida = vidaMax;
        anguloWander = Random.Range(0f, 360f);
        CrearFSM();
    }

    private void Start()
    {
        IniciarVelocidad(DireccionAleatoria() * velocidadInicial);
        fsm.Iniciar(EstadoBoid.Flocking);
    }

    private void Update()
    {
        fsm.Actualizar();
        AplicarMovimiento();

        if (EstadoActual != EstadoBoid.Dead && EstadoActual != EstadoBoid.Respawn)
            CorregirSuperposicion();
    }

    private void CorregirSuperposicion()
    {
        float distanciaMinima = radioCuerpo * 2f;

        foreach (Boid otro in Percepcion.Vecinos)
        {
            if (otro == null || otro.EstadoActual == EstadoBoid.Respawn) continue;

            Vector3 diferencia = transform.position - otro.transform.position;
            diferencia.y = 0f;
            float distancia = diferencia.magnitude;
            if (distancia >= distanciaMinima || distancia < 0.001f) continue;

            float empuje = otro.EstadoActual == EstadoBoid.Dead ? distanciaMinima - distancia : (distanciaMinima - distancia) * 0.5f;
            transform.position += diferencia / distancia * empuje;
        }

        if (Limites.Instancia != null)
            transform.position = Limites.Instancia.Restringir(transform.position);
    }

    private void CrearFSM()
    {
        fsm = new MaquinaEstados<EstadoBoid>();
        fsm.OnCambioEstado += estado => OnCambioEstado?.Invoke(estado);

        EstadoRespawn respawn = new EstadoRespawn(this);

        fsm.AgregarEstado(EstadoBoid.Flocking, new EstadoFlocking(this));
        fsm.AgregarEstado(EstadoBoid.Evade, new EstadoEvade(this));
        fsm.AgregarEstado(EstadoBoid.SeekObject, new EstadoSeekObject(this));
        fsm.AgregarEstado(EstadoBoid.Consume, new EstadoConsume(this));
        fsm.AgregarEstado(EstadoBoid.Dead, new EstadoDead(this));
        fsm.AgregarEstado(EstadoBoid.Respawn, respawn);

        System.Func<bool> muerto = () => EstaMuerto;
        System.Func<bool> veCazador = () => Percepcion.Cazador != null;
        System.Func<bool> veObjeto = () => Percepcion.ObjetoMasCercano != null;
        System.Func<bool> sinObjetivo = () => ObjetivoActual == null;

        fsm.AgregarTransicion(EstadoBoid.Flocking, EstadoBoid.Dead, muerto);
        fsm.AgregarTransicion(EstadoBoid.Flocking, EstadoBoid.Evade, veCazador);
        fsm.AgregarTransicion(EstadoBoid.Flocking, EstadoBoid.SeekObject, veObjeto);

        fsm.AgregarTransicion(EstadoBoid.Evade, EstadoBoid.Dead, muerto);
        fsm.AgregarTransicion(EstadoBoid.Evade, EstadoBoid.SeekObject, () => !veCazador() && veObjeto());
        fsm.AgregarTransicion(EstadoBoid.Evade, EstadoBoid.Flocking, () => !veCazador());

        fsm.AgregarTransicion(EstadoBoid.SeekObject, EstadoBoid.Dead, muerto);
        fsm.AgregarTransicion(EstadoBoid.SeekObject, EstadoBoid.Evade, veCazador);
        fsm.AgregarTransicion(EstadoBoid.SeekObject, EstadoBoid.Consume, LlegoAlObjetivo);
        fsm.AgregarTransicion(EstadoBoid.SeekObject, EstadoBoid.Flocking, sinObjetivo);

        fsm.AgregarTransicion(EstadoBoid.Consume, EstadoBoid.Dead, muerto);
        fsm.AgregarTransicion(EstadoBoid.Consume, EstadoBoid.Evade, veCazador);
        fsm.AgregarTransicion(EstadoBoid.Consume, EstadoBoid.Flocking, sinObjetivo);

        fsm.AgregarTransicion(EstadoBoid.Dead, EstadoBoid.Respawn, () => FueRecolectado);

        fsm.AgregarTransicion(EstadoBoid.Respawn, EstadoBoid.Flocking, () => respawn.Terminado);
    }

    public Vector3 FuerzaSeparacion()
    {
        return Flocking.Separacion(this, Percepcion.Vecinos, radioSeparacion) * pesoSeparacion;
    }

    public Vector3 FuerzaSeparacionEvade()
    {
        return Flocking.Separacion(this, Percepcion.Vecinos, radioSeparacion) * pesoSeparacionEvade;
    }

    public Vector3 FuerzaAlineacion()
    {
        return Flocking.Alineacion(this, Percepcion.Vecinos) * pesoAlineacion;
    }

    public Vector3 FuerzaCohesion()
    {
        return Flocking.Cohesion(this, Percepcion.Vecinos) * pesoCohesion;
    }

    public Vector3 FuerzaWander()
    {
        return Steering.Wander(this, ref anguloWander, distanciaWander, radioWander, variacionWander) * pesoWander;
    }

    public Vector3 FuerzaEvade(Agente cazador)
    {
        return Steering.Evade(this, cazador, prediccionMax) * pesoEvade;
    }

    public Vector3 FuerzaArrive(ObjetoInteres objeto)
    {
        return Steering.Arrive(this, objeto.transform.position, radioFrenado, DistanciaParada(objeto)) * pesoArrive;
    }

    public Vector3 FuerzaLimites()
    {
        if (Limites.Instancia == null) return Vector3.zero;
        return Limites.Instancia.FuerzaParaVolver(this) * pesoLimites;
    }

    public Vector3 FuerzaEvitarObjetos()
    {
        Vector3 alejarse = Vector3.zero;

        foreach (ObjetoInteres objeto in Percepcion.Objetos)
        {
            if (objeto == null) continue;

            Vector3 diferencia = transform.position - objeto.transform.position;
            diferencia.y = 0f;
            float distancia = diferencia.magnitude;
            float distanciaMinima = objeto.Radio + radioCuerpo + margenObjeto * 0.5f;
            if (distancia > distanciaMinima || distancia < 0.001f) continue;

            alejarse += diferencia.normalized * (distanciaMinima - distancia) / distanciaMinima;
        }

        return Steering.HaciaDireccion(this, alejarse) * pesoEvitarObjetos;
    }

    public float DistanciaParada(ObjetoInteres objeto)
    {
        return objeto.Radio + radioCuerpo + margenObjeto;
    }

    private bool LlegoAlObjetivo()
    {
        if (ObjetivoActual == null) return false;
        return DistanciaPlana(ObjetivoActual.transform.position) <= DistanciaParada(ObjetivoActual) + toleranciaLlegada;
    }

    public void RecibirGolpes(int golpes)
    {
        if (EstaMuerto) return;

        vida = Mathf.Max(0, vida - golpes);
        OnGolpeado?.Invoke();
    }

    public void Recolectar()
    {
        if (EstaMuerto) FueRecolectado = true;
    }

    public void Ocultar()
    {
        MostrarCuerpo(false);
    }

    public void Reaparecer()
    {
        transform.position = Limites.Instancia.PosicionLibre(distanciaRespawnAlBorde, radioCuerpo * 2f, Percepcion.CapasOcupadas);
        vida = vidaMax;
        FueRecolectado = false;
        ObjetivoActual = null;
        IniciarVelocidad(DireccionAleatoria() * velocidadInicial);
        MostrarCuerpo(true);
    }

    private void MostrarCuerpo(bool mostrar)
    {
        foreach (Renderer r in renderers)
            r.enabled = mostrar;
        colisionador.enabled = mostrar;
    }

    private Vector3 DireccionAleatoria()
    {
        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Vector3.forward;
    }

    private void OnValidate()
    {
        radioVecinos = Mathf.Max(radioVecinos, radioSeparacion + 0.1f);
        velocidadInicial = Mathf.Clamp(velocidadInicial, 0f, velocidadMax);
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioSeparacion);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioVecinos);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioVision);
    }
}
