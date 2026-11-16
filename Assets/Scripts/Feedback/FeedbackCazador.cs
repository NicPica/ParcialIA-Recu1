using UnityEngine;

[RequireComponent(typeof(Cazador))]
public class FeedbackCazador : MonoBehaviour
{
    [SerializeField] private Renderer cuerpo;
    [SerializeField] private EtiquetaEstado etiqueta;
    [SerializeField] private LineRenderer lineaAtaque;

    [Header("Colores por estado")]
    [SerializeField] private Color colorPatrol = new Color(0.3f, 0.5f, 1f);
    [SerializeField] private Color colorHunt = Color.red;
    [SerializeField] private Color colorGather = new Color(0.7f, 0.3f, 0.9f);
    [SerializeField] private Color colorRest = Color.gray;

    [Header("Ataques")]
    [SerializeField] private Color colorMelee = Color.red;
    [SerializeField] private Color colorRango = Color.yellow;
    [SerializeField] private float duracionLinea = 0.2f;

    [Header("Texto")]
    [SerializeField] private float intervaloTexto = 0.1f;

    private Cazador cazador;
    private Material material;
    private Color colorActual;
    private float tiempoLinea;
    private float tiempoTexto;

    private void Awake()
    {
        cazador = GetComponent<Cazador>();
        material = cuerpo.material;
        if (lineaAtaque != null) lineaAtaque.enabled = false;
    }

    private void OnEnable()
    {
        cazador.OnCambioEstado += CambiarEstado;
        cazador.OnAtaque += MostrarAtaque;
    }

    private void OnDisable()
    {
        cazador.OnCambioEstado -= CambiarEstado;
        cazador.OnAtaque -= MostrarAtaque;
    }

    private void Update()
    {
        if (tiempoLinea > 0f)
        {
            tiempoLinea -= Time.deltaTime;
            if (tiempoLinea <= 0f) lineaAtaque.enabled = false;
        }

        tiempoTexto -= Time.deltaTime;
        if (tiempoTexto <= 0f)
        {
            tiempoTexto = intervaloTexto;
            ActualizarTexto();
        }
    }

    private void CambiarEstado(EstadoCazador estado)
    {
        colorActual = ColorDe(estado);
        material.color = colorActual;
        ActualizarTexto();
    }

    private void MostrarAtaque(Boid presa, bool cuerpoACuerpo)
    {
        if (lineaAtaque == null) return;

        Color color = cuerpoACuerpo ? colorMelee : colorRango;
        lineaAtaque.startColor = color;
        lineaAtaque.endColor = color;
        lineaAtaque.SetPosition(0, transform.position + Vector3.up);
        lineaAtaque.SetPosition(1, presa.transform.position + Vector3.up * 0.4f);
        lineaAtaque.enabled = true;
        tiempoLinea = duracionLinea;
    }

    private void ActualizarTexto()
    {
        if (etiqueta == null) return;

        string objetivo = "";
        if (cazador.Presa != null) objetivo = "Presa: " + cazador.Presa.name;
        else if (cazador.BoidARecolectar != null) objetivo = "Recolecta: " + cazador.BoidARecolectar.name;

        string texto = $"{cazador.EstadoActual}\nEnergia {cazador.Energy:0}/{cazador.MaxEnergy:0}\n{objetivo}";
        etiqueta.Mostrar(texto, colorActual);
    }

    private Color ColorDe(EstadoCazador estado)
    {
        switch (estado)
        {
            case EstadoCazador.Hunt: return colorHunt;
            case EstadoCazador.Gather: return colorGather;
            case EstadoCazador.Rest: return colorRest;
            default: return colorPatrol;
        }
    }
}
