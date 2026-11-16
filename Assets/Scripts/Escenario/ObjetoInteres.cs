using UnityEngine;

public class ObjetoInteres : MonoBehaviour
{
    public static int Activos { get; private set; }

    [SerializeField] private int vidaMax = 6;
    [SerializeField] private float radio = 0.5f;

    private int vida;

    public int Vida => vida;
    public int VidaMax => vidaMax;
    public float Radio => radio;

    public event System.Action OnDanado;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarContador()
    {
        Activos = 0;
    }

    private void Awake()
    {
        vida = vidaMax;
    }

    private void OnEnable()
    {
        Activos++;
    }

    private void OnDisable()
    {
        Activos--;
    }

    public void RecibirDanio(int danio)
    {
        if (vida <= 0) return;

        vida -= danio;
        OnDanado?.Invoke();

        if (vida <= 0)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.8f, 0f);
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
