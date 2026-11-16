using UnityEngine;

public abstract class Agente : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] protected float velocidadMax = 4f;
    [SerializeField] protected float fuerzaMax = 12f;
    [SerializeField] private bool restringirALimites = true;

    private Vector3 velocidad;
    private Vector3 fuerzaTotal;

    public Vector3 Velocidad => velocidad;
    public float VelocidadMax => velocidadMax;
    public float LimiteVelocidad { get; private set; }

    protected virtual void Awake()
    {
        LimiteVelocidad = velocidadMax;
    }

    public void CambiarLimiteVelocidad(float limite)
    {
        LimiteVelocidad = limite;
    }

    public void AgregarFuerza(Vector3 fuerza)
    {
        fuerzaTotal += fuerza;
    }

    public void Detener()
    {
        velocidad = Vector3.zero;
        fuerzaTotal = Vector3.zero;
    }

    public float DistanciaPlana(Vector3 punto)
    {
        Vector3 diferencia = punto - transform.position;
        diferencia.y = 0f;
        return diferencia.magnitude;
    }

    protected void IniciarVelocidad(Vector3 velocidadInicial)
    {
        velocidad = velocidadInicial;
    }

    protected void AplicarMovimiento()
    {
        Vector3 fuerza = Vector3.ClampMagnitude(fuerzaTotal, fuerzaMax);
        fuerza.y = 0f;
        fuerzaTotal = Vector3.zero;

        velocidad = Vector3.ClampMagnitude(velocidad + fuerza * Time.deltaTime, LimiteVelocidad);

        Vector3 posicion = transform.position + velocidad * Time.deltaTime;
        if (restringirALimites && Limites.Instancia != null)
            posicion = Limites.Instancia.Restringir(posicion);
        transform.position = posicion;

        if (velocidad.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(velocidad);
    }
}
