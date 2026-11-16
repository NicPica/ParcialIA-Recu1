using UnityEngine;

public class EtiquetaEstado : MonoBehaviour
{
    [SerializeField] private TextMesh texto;

    private Transform camara;

    private void Start()
    {
        if (Camera.main != null)
            camara = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (camara != null)
            texto.transform.rotation = camara.rotation;
    }

    public void Mostrar(string mensaje, Color color)
    {
        texto.text = mensaje;
        texto.color = color;
    }
}
