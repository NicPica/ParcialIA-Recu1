using UnityEngine;

public class Limites : MonoBehaviour
{
    public static Limites Instancia { get; private set; }

    [SerializeField] private float ancho = 40f;
    [SerializeField] private float largo = 40f;
    [SerializeField] private float margen = 3f;

    private Vector3 Minimo => transform.position - new Vector3(ancho, 0f, largo) * 0.5f;
    private Vector3 Maximo => transform.position + new Vector3(ancho, 0f, largo) * 0.5f;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    private void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    public bool Contiene(Vector3 punto)
    {
        Vector3 min = Minimo;
        Vector3 max = Maximo;
        return punto.x >= min.x && punto.x <= max.x && punto.z >= min.z && punto.z <= max.z;
    }

    public Vector3 Restringir(Vector3 punto)
    {
        Vector3 min = Minimo;
        Vector3 max = Maximo;
        punto.x = Mathf.Clamp(punto.x, min.x, max.x);
        punto.z = Mathf.Clamp(punto.z, min.z, max.z);
        return punto;
    }

    public Vector3 PosicionAleatoria(float distanciaAlBorde)
    {
        Vector3 min = Minimo;
        Vector3 max = Maximo;
        return new Vector3(
            Random.Range(min.x + distanciaAlBorde, max.x - distanciaAlBorde),
            transform.position.y,
            Random.Range(min.z + distanciaAlBorde, max.z - distanciaAlBorde));
    }

    public Vector3 PosicionLibre(float distanciaAlBorde, float radio, int capas, int intentos = 15)
    {
        Vector3 posicion = PosicionAleatoria(distanciaAlBorde);
        for (int i = 0; i < intentos; i++)
        {
            if (!Physics.CheckSphere(posicion, radio, capas, QueryTriggerInteraction.Collide))
                return posicion;
            posicion = PosicionAleatoria(distanciaAlBorde);
        }
        return posicion;
    }

    public Vector3[] Esquinas(float distanciaAlBorde)
    {
        Vector3 min = Minimo + new Vector3(distanciaAlBorde, 0f, distanciaAlBorde);
        Vector3 max = Maximo - new Vector3(distanciaAlBorde, 0f, distanciaAlBorde);
        return new Vector3[]
        {
            new Vector3(min.x, transform.position.y, min.z),
            new Vector3(min.x, transform.position.y, max.z),
            new Vector3(max.x, transform.position.y, max.z),
            new Vector3(max.x, transform.position.y, min.z)
        };
    }

    public Vector3 FuerzaParaVolver(Agente agente)
    {
        Vector3 posicion = agente.transform.position;
        Vector3 min = Minimo;
        Vector3 max = Maximo;
        Vector3 deseada = agente.Velocidad;
        bool cercaDelBorde = false;

        if (posicion.x < min.x + margen) { deseada.x = agente.LimiteVelocidad; cercaDelBorde = true; }
        else if (posicion.x > max.x - margen) { deseada.x = -agente.LimiteVelocidad; cercaDelBorde = true; }

        if (posicion.z < min.z + margen) { deseada.z = agente.LimiteVelocidad; cercaDelBorde = true; }
        else if (posicion.z > max.z - margen) { deseada.z = -agente.LimiteVelocidad; cercaDelBorde = true; }

        if (!cercaDelBorde) return Vector3.zero;
        return Steering.HaciaDireccion(agente, deseada);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(ancho, 0.1f, largo));
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(ancho - margen * 2f, 0.1f, largo - margen * 2f));
    }
}
