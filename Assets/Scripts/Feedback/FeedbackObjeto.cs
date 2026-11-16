using UnityEngine;

[RequireComponent(typeof(ObjetoInteres))]
public class FeedbackObjeto : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float escalaMinima = 0.4f;

    private ObjetoInteres objeto;
    private Vector3 escalaInicial;

    private void Awake()
    {
        objeto = GetComponent<ObjetoInteres>();
        escalaInicial = visual.localScale;
    }

    private void OnEnable()
    {
        objeto.OnDanado += Actualizar;
    }

    private void OnDisable()
    {
        objeto.OnDanado -= Actualizar;
    }

    private void Actualizar()
    {
        float porcentaje = (float)objeto.Vida / objeto.VidaMax;
        visual.localScale = escalaInicial * Mathf.Lerp(escalaMinima, 1f, porcentaje);
    }
}
