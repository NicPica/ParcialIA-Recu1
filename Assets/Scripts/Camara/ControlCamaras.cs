using UnityEngine;
using UnityEngine.InputSystem;

public class ControlCamaras : MonoBehaviour
{
    private enum Modo { General, Cazador, Boid }

    [SerializeField] private Vector3 distanciaSeguimiento = new Vector3(0f, 10f, -8f);
    [SerializeField] private float suavizado = 5f;
    [SerializeField] private bool mostrarAyuda = true;

    private Modo modo = Modo.General;
    private Vector3 posicionGeneral;
    private Quaternion rotacionGeneral;
    private Transform objetivo;
    private int indiceBoid = -1;

    private void Start()
    {
        posicionGeneral = transform.position;
        rotacionGeneral = transform.rotation;
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        if (teclado.digit1Key.wasPressedThisFrame)
            modo = Modo.General;
        else if (teclado.digit2Key.wasPressedThisFrame)
            SeguirCazador();
        else if (teclado.digit3Key.wasPressedThisFrame)
            SeguirSiguienteBoid();
    }

    private void LateUpdate()
    {
        Vector3 posicion = posicionGeneral;
        Quaternion rotacion = rotacionGeneral;

        if (modo != Modo.General && objetivo != null)
        {
            posicion = objetivo.position + distanciaSeguimiento;
            rotacion = Quaternion.LookRotation(objetivo.position - posicion);
        }

        float t = suavizado * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, posicion, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, t);
    }

    private void SeguirCazador()
    {
        Cazador cazador = FindFirstObjectByType<Cazador>();
        if (cazador == null) return;

        objetivo = cazador.transform;
        modo = Modo.Cazador;
    }

    private void SeguirSiguienteBoid()
    {
        Boid[] boids = FindObjectsByType<Boid>(FindObjectsSortMode.InstanceID);
        if (boids.Length == 0) return;

        indiceBoid = (indiceBoid + 1) % boids.Length;
        objetivo = boids[indiceBoid].transform;
        modo = Modo.Boid;
    }

    private void OnGUI()
    {
        if (!mostrarAyuda) return;

        string actual = modo == Modo.General || objetivo == null ? "General" : objetivo.name;
        GUI.Box(new Rect(10f, 10f, 330f, 70f), "");
        GUI.Label(new Rect(20f, 15f, 320f, 65f), $"Toca los numeros para cambiar de camara:\n1 General | 2 Cazador | 3 Siguiente boid\nCamara actual: {actual}");
    }
}
