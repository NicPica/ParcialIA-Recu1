using System.Collections.Generic;

public class MaquinaEstados<T>
{
    private class Transicion
    {
        public T destino;
        public System.Func<bool> condicion;
    }

    private readonly Dictionary<T, IEstado> estados = new Dictionary<T, IEstado>();
    private readonly Dictionary<T, List<Transicion>> transiciones = new Dictionary<T, List<Transicion>>();
    private IEstado estadoActual;

    public T Actual { get; private set; }

    public event System.Action<T> OnCambioEstado;

    public void AgregarEstado(T clave, IEstado estado)
    {
        estados[clave] = estado;
        transiciones[clave] = new List<Transicion>();
    }

    public void AgregarTransicion(T desde, T hacia, System.Func<bool> condicion)
    {
        transiciones[desde].Add(new Transicion { destino = hacia, condicion = condicion });
    }

    public void Iniciar(T clave)
    {
        CambiarEstado(clave);
    }

    public void Actualizar()
    {
        if (estadoActual == null) return;

        foreach (Transicion transicion in transiciones[Actual])
        {
            if (transicion.condicion())
            {
                CambiarEstado(transicion.destino);
                break;
            }
        }

        estadoActual.Ejecutar();
    }

    private void CambiarEstado(T clave)
    {
        estadoActual?.Salir();
        Actual = clave;
        estadoActual = estados[clave];
        estadoActual.Entrar();
        OnCambioEstado?.Invoke(clave);
    }
}
