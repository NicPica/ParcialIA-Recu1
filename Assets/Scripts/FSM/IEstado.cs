public abstract class Estado<T>
{
    protected MaquinaEstados<T> fsm;

    public void AsignarFSM(MaquinaEstados<T> maquina) => fsm = maquina;

    public abstract void Entrar();
    public abstract void Ejecutar();
    public abstract void Salir();
}
