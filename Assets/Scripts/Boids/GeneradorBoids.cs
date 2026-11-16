using UnityEngine;

public class GeneradorBoids : MonoBehaviour
{
    [SerializeField] private Boid prefabBoid;
    [SerializeField] private int cantidad = 10;
    [SerializeField] private float radioAparicion = 6f;

    private void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 desvio = Random.insideUnitCircle * radioAparicion;
            Vector3 posicion = transform.position + new Vector3(desvio.x, 0f, desvio.y);
            Boid boid = Instantiate(prefabBoid, posicion, Quaternion.identity, transform);
            boid.name = "Boid " + (i + 1);
        }
    }

    private void OnValidate()
    {
        cantidad = Mathf.Max(6, cantidad);
    }
}
