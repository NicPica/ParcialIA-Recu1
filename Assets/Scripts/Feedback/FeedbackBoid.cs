using UnityEngine;

[RequireComponent(typeof(Boid))]
public class FeedbackBoid : MonoBehaviour
{
    [SerializeField] private Renderer cuerpo;
    [SerializeField] private EtiquetaEstado etiqueta;

    [Header("Colores por estado")]
    [SerializeField] private Color colorFlocking = new Color(0.3f, 0.9f, 1f);
    [SerializeField] private Color colorEvade = Color.yellow;
    [SerializeField] private Color colorSeekObject = Color.green;
    [SerializeField] private Color colorConsume = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color colorDead = new Color(0.25f, 0.25f, 0.25f);
    [SerializeField] private Color colorRespawn = Color.white;

    [Header("Golpe")]
    [SerializeField] private Color colorGolpe = Color.red;
    [SerializeField] private float duracionGolpe = 0.15f;

    private Boid boid;
    private Material material;
    private Color colorActual;
    private float tiempoGolpe;

    private void Awake()
    {
        boid = GetComponent<Boid>();
        material = cuerpo.material;
    }

    private void OnEnable()
    {
        boid.OnCambioEstado += CambiarEstado;
        boid.OnGolpeado += MostrarGolpe;
    }

    private void OnDisable()
    {
        boid.OnCambioEstado -= CambiarEstado;
        boid.OnGolpeado -= MostrarGolpe;
    }

    private void Update()
    {
        if (tiempoGolpe <= 0f) return;

        tiempoGolpe -= Time.deltaTime;
        if (tiempoGolpe <= 0f)
            material.color = colorActual;
    }

    private void CambiarEstado(EstadoBoid estado)
    {
        colorActual = ColorDe(estado);
        if (tiempoGolpe <= 0f)
            material.color = colorActual;
        ActualizarTexto();
    }

    private void MostrarGolpe()
    {
        tiempoGolpe = duracionGolpe;
        material.color = colorGolpe;
        ActualizarTexto();
    }

    private void ActualizarTexto()
    {
        if (etiqueta == null) return;
        etiqueta.Mostrar($"{boid.EstadoActual}\n{boid.Vida}/{boid.VidaMax}", colorActual);
    }

    private Color ColorDe(EstadoBoid estado)
    {
        switch (estado)
        {
            case EstadoBoid.Evade: return colorEvade;
            case EstadoBoid.SeekObject: return colorSeekObject;
            case EstadoBoid.Consume: return colorConsume;
            case EstadoBoid.Dead: return colorDead;
            case EstadoBoid.Respawn: return colorRespawn;
            default: return colorFlocking;
        }
    }
}
