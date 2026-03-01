using UnityEngine;

public abstract class ComportamientoGuardia : MonoBehaviour
{
    [HideInInspector] public GuardiaCerebro cerebro;

    public virtual void Entrar() { }
    public virtual void Ejecutar() { }
    public virtual void Salir() { }
}