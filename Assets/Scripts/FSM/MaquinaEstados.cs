using System.Collections.Generic;
using UnityEngine;

public class MaquinaEstados<T>
{
    private readonly Dictionary<T, Estado<T>> estados = new Dictionary<T, Estado<T>>();
    private Estado<T> estadoActual;

    public T Actual { get; private set; }

    public event System.Action<T> OnCambioEstado;

    public void AgregarEstado(T clave, Estado<T> estado)
    {
        estado.AsignarFSM(this);
        estados[clave] = estado;
    }

    public void CambiarEstado(T clave)
    {
        if (!estados.ContainsKey(clave))
        {
            Debug.LogError("Falta el estado " + clave);
            return;
        }

        estadoActual?.Salir();
        Actual = clave;
        estadoActual = estados[clave];
        estadoActual.Entrar();
        OnCambioEstado?.Invoke(clave);
    }

    public void Actualizar()
    {
        if (estadoActual != null) estadoActual.Ejecutar();
    }
}
